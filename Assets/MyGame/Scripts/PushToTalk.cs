using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif


/// <summary>
/// Discord-Quality Push-to-Talk Microphone Input
/// Implements: High-Pass Filter, Noise Gate (Hysteresis),
///             AGC, Soft Compressor, True Peak Limiter
/// Audio Thread: OnAudioFilterRead (zero garbage, no Update overhead)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DiscordQualityMicInput : MonoBehaviour
{
    
    [Header("─── Push-To-Talk ──────────────────────")]
    [SerializeField] 
    private InputActionProperty pushToTalkAction;


    [Header("─── Audio Mixer ────────────────────────")]
    [Tooltip("Name des AudioMixers in deinem Resources-Ordner (ohne Dateierweiterung).")]
    public string mixerResourceName = "MicMixer";
    [Tooltip("Name des Mixer-Channels (Standard: Master).")]
    public string mixerGroupName    = "Master";

    [Header("─── Sample Rate ───────────────────────")]
    [Tooltip("48000 = Discord Standard. Mikrofon muss das unterstützen.")]
    public int targetSampleRate = 48000;

    [Header("─── Noise Gate ────────────────────────")]
    [Tooltip("Ab diesem RMS-Wert öffnet das Gate.")]
    public float gateOpenThreshold  = 0.008f;
    [Tooltip("Unter diesem RMS-Wert schließt das Gate (Hysterese verhindert Flattern).")]
    public float gateCloseThreshold = 0.003f;
    [Tooltip("Attack-Zeit in Sekunden (wie schnell das Gate aufgeht). Discord: ~10 ms")]
    public float attackTime  = 0.010f;
    [Tooltip("Release-Zeit in Sekunden (wie lang es nachklingt). Discord: ~150 ms")]
    public float releaseTime = 0.150f;

    [Header("─── AGC (Automatic Gain Control) ──────")]
    public bool  enableAGC    = true;
    [Tooltip("Ziel-RMS-Pegel. 0.25 = angenehme Gesprächslautstärke")]
    public float targetRMS    = 0.25f;
    [Tooltip("Minimale Verstärkung (verhindert zu leises Übersteuern)")]
    public float minGain      = 0.5f;
    [Tooltip("Maximale Verstärkung (verhindert Rauschen aus dem Nichts)")]
    public float maxGain      = 6.0f;
    [Tooltip("Wie schnell die AGC reagiert (kleinerer Wert = träger = smoother)")]
    public float agcSpeed     = 1.5f;

    [Header("─── Soft Compressor ───────────────────")]
    public bool  enableCompressor      = true;
    [Tooltip("Ab diesem Sample-Wert greift die Kompression (0.0 - 1.0)")]
    public float compressorThreshold   = 0.45f;
    [Tooltip("Kompressionsverhältnis. Discord nutzt ~4:1")]
    public float compressorRatio       = 4.0f;
    [Tooltip("Knee-Breite: weicher Übergang statt harter Schwelle")]
    [Range(0f, 0.2f)]
    public float kneeWidth             = 0.05f;

    [Header("─── High-Pass Filter ───────────────────")]
    [Tooltip("Cutoff-Frequenz des High-Pass Filters in Hz. 80 Hz entfernt Rumpeln & Tastatur.")]
    public float hpfCutoffHz = 80f;

    // ── Private State ──────────────────────────────────────────────────────────
    private AudioSource audioSource;
    private string      micName;
    private bool        isTalking  = false;

    // Gate state (audio thread)
    private float gateGain = 0f;
    private bool  gateOpen = false;

    // AGC state (audio thread)
    private float currentGain = 1f;

    // High-Pass Filter state – first-order IIR, per channel
    // y[n] = alpha * (y[n-1] + x[n] - x[n-1])
    private float   hpAlpha;
    private float[] hpPrevX;   // previous input sample per channel
    private float[] hpPrevY;   // previous output sample per channel

    // RMS sliding window (audio thread, pre-allocated, no GC)
    private const int RMS_WINDOW_SIZE = 20;
    private float[]   rmsRing = new float[RMS_WINDOW_SIZE];
    private int       rmsHead = 0;

    // Shared between audio thread and main thread (volatile for thread safety)
    private volatile float sharedRMS = 0f;
    private volatile float sharedGain = 1f;

    // Gecachter Sample Rate – darf NICHT im Audio-Thread abgefragt werden
    private int cachedOutputSampleRate = 48000;

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        #endif

        audioSource = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop         = true;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake  = false;

        // Sample Rate einmalig im Main Thread cachen (darf NICHT im Audio-Thread abgefragt werden)
        cachedOutputSampleRate = AudioSettings.outputSampleRate;

        LoadAndAssignMixer();

        // Stereo-safe: 2 Channels initialisieren
        hpPrevX = new float[2];
        hpPrevY = new float[2];
        RecalculateHPF();

        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            Debug.Log("Mikrofon gefunden: " + micName);
        }
        else
        {
            Debug.LogError("Kein Mikrofon gefunden!");
        }
    }

    void Update()
    {
        
        if (pushToTalkAction.action == null)
        return;

        Debug.Log(pushToTalkAction.action.IsPressed());

        bool pressed = pushToTalkAction.action.IsPressed();

        if (pressed && !isTalking)
            StartTalking();

        if (!pressed && isTalking)
            StopTalking();
    }
    void OnEnable()
    {
        pushToTalkAction.action?.Enable();
    }

    void OnDisable()
    {
        pushToTalkAction.action?.Disable();
    }

    // ── Mixer Setup ────────────────────────────────────────────────────────────

    /// <summary>
    /// Lädt den AudioMixer aus dem Resources-Ordner und weist ihn der AudioSource zu.
    /// Voraussetzung: Assets/Resources/MicMixer.mixer existiert.
    /// </summary>
    void LoadAndAssignMixer()
    {
        AudioMixer mixer = Resources.Load<AudioMixer>(mixerResourceName);

        if (mixer == null)
        {
            Debug.LogWarning($"AudioMixer '{mixerResourceName}' nicht in Resources gefunden.\n" +
                             "→ Erstelle ihn unter Assets/Resources/" + mixerResourceName + ".mixer");
            return;
        }

        AudioMixerGroup[] groups = mixer.FindMatchingGroups(mixerGroupName);

        if (groups.Length == 0)
        {
            Debug.LogWarning($"Mixer-Group '{mixerGroupName}' nicht im Mixer '{mixerResourceName}' gefunden.");
            return;
        }

        audioSource.outputAudioMixerGroup = groups[0];
        Debug.Log($"AudioMixer '{mixerResourceName}' → Group '{mixerGroupName}' zugewiesen.");
    }

    // ── Push-To-Talk ───────────────────────────────────────────────────────────


    void StartTalking()
    {
        isTalking = true;
        StartCoroutine(StartMicCoroutine());
    }

    IEnumerator StartMicCoroutine()
    {
        Microphone.GetDeviceCaps(micName, out int minFreq, out int maxFreq);

        // Wähle den besten unterstützten Sample-Rate
        int freq;
        if (maxFreq == 0)
            freq = targetSampleRate;          // Mikrofon unterstützt beliebige Rate
        else
            freq = Mathf.Clamp(targetSampleRate, minFreq, maxFreq);

        audioSource.clip = Microphone.Start(micName, true, 10, freq);

        // Warten bis der erste echte Sample da ist
        while (Microphone.GetPosition(micName) <= 0)
            yield return null;

        // Sync Position – verhindert initialen Echo-/Stutter-Bug
        audioSource.timeSamples = Microphone.GetPosition(micName);
        audioSource.Play();

        // State zurücksetzen für einen sauberen Start
        gateGain    = 0f;
        gateOpen    = false;
        currentGain = 1f;
        System.Array.Clear(rmsRing,  0, rmsRing.Length);
        System.Array.Clear(hpPrevX, 0, hpPrevX.Length);
        System.Array.Clear(hpPrevY, 0, hpPrevY.Length);

        Debug.Log($"Aufnahme gestartet @ {freq} Hz");
    }

    void StopTalking()
    {
        isTalking = false;
        gateGain  = 0f;
        gateOpen  = false;
        audioSource.Stop();
        Microphone.End(micName);
        Debug.Log("Aufnahme gestoppt");
    }

    // ── Audio DSP Pipeline (Audio Thread – kein GC erlaubt!) ──────────────────
    // Reihenfolge: HPF → AGC → Kompressor → Limiter → Noise Gate

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isTalking) return;

        int sampleCount   = data.Length / channels;
        int outputSR      = cachedOutputSampleRate;

        // Zeitdauer dieses Buffers in Sekunden (für Gate Attack/Release)
        float bufferDuration = (float)sampleCount / outputSR;

        // ── 1. RMS berechnen (vor jeder Bearbeitung) ──────────────────────────
        float sumSq = 0f;
        for (int i = 0; i < data.Length; i += channels)
            sumSq += data[i] * data[i];
        float rms = Mathf.Sqrt(sumSq / sampleCount);

        // Sliding-Window RMS (glatt, nicht zitternd)
        rmsRing[rmsHead % RMS_WINDOW_SIZE] = rms;
        rmsHead++;
        float smoothRMS = GetRingAverage();
        sharedRMS = smoothRMS; // für Debugging im Hauptthread

        // ── 2. Noise Gate (Hysterese) ─────────────────────────────────────────
        // Zwei verschiedene Schwellen → kein Flattern beim Grenzwert
        if (!gateOpen && smoothRMS > gateOpenThreshold)
            gateOpen = true;
        else if (gateOpen && smoothRMS < gateCloseThreshold)
            gateOpen = false;

        float gateTarget = gateOpen ? 1f : 0f;
        float gateStep   = bufferDuration / (gateOpen ? attackTime : releaseTime);
        gateGain = Mathf.MoveTowards(gateGain, gateTarget, gateStep);

        // ── 3. AGC Gain berechnen ─────────────────────────────────────────────
        if (enableAGC && smoothRMS > 0.0001f)
        {
            float desiredGain = targetRMS / smoothRMS;
            desiredGain = Mathf.Clamp(desiredGain, minGain, maxGain);
            // Träge Anpassung – verhindert Pumpen-Effekt
            float lerpSpeed = 1f - Mathf.Exp(-agcSpeed * bufferDuration);
            currentGain = Mathf.Lerp(currentGain, desiredGain, lerpSpeed);
        }
        sharedGain = currentGain;

        // ── 4. Sample-für-Sample Verarbeitung ─────────────────────────────────
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int ch = 0; ch < channels && ch < 2; ch++)
            {
                float x = data[i + ch];

                // 4a. High-Pass Filter (entfernt Rumpeln & Tastaturgeräusche)
                //     y[n] = alpha * (y[n-1] + x[n] - x[n-1])
                float y = hpAlpha * (hpPrevY[ch] + x - hpPrevX[ch]);
                hpPrevX[ch] = x;
                hpPrevY[ch] = y;
                x = y;

                // 4b. AGC
                if (enableAGC)
                    x *= currentGain;

                // 4c. Soft-Knee Kompressor
                if (enableCompressor)
                    x = SoftKneeCompress(x);

                // 4d. True Peak Limiter (kein hartes Clipping)
                x = SoftLimit(x, 0.95f);

                // 4e. Noise Gate anwenden
                x *= gateGain;

                data[i + ch] = x;
            }
        }
    }

    // ── DSP Hilfsfunktionen ────────────────────────────────────────────────────

    /// <summary>
    /// Soft-Knee Kompressor – glatter Übergang statt harter Schwelle.
    /// Klingt wie professionelle Broadcast-Kompression.
    /// </summary>
    float SoftKneeCompress(float sample)
    {
        float sign   = Mathf.Sign(sample);
        float abs    = Mathf.Abs(sample);
        float kneeStart = compressorThreshold - kneeWidth * 0.5f;
        float kneeEnd   = compressorThreshold + kneeWidth * 0.5f;

        if (abs <= kneeStart)
            return sample; // Kein Eingriff unterhalb der Knee

        if (abs >= kneeEnd)
        {
            // Vollständige Kompression
            float over = abs - compressorThreshold;
            float compressed = compressorThreshold + over / compressorRatio;
            return sign * compressed;
        }

        // Übergangsbereich (Knee) – quadratische Interpolation
        float t = (abs - kneeStart) / kneeWidth;
        float kneeGain = 1f + (1f / compressorRatio - 1f) * t * t * 0.5f;
        return sign * abs * kneeGain;
    }

    /// <summary>
    /// Soft Limiter – verhindert hartes Clipping via Tanh-Approximation.
    /// Klingt natürlicher als Mathf.Clamp.
    /// </summary>
    float SoftLimit(float x, float ceiling)
    {
        float scaled = x / ceiling;
        // Tanh-Approximation: schnell, kein GC
        float tanh = scaled / (1f + Mathf.Abs(scaled));
        return tanh * ceiling;
    }

    /// <summary>Gleitender Durchschnitt des RMS-Ringpuffers</summary>
    float GetRingAverage()
    {
        float sum = 0f;
        int count = Mathf.Min(rmsHead, RMS_WINDOW_SIZE);
        for (int i = 0; i < count; i++) sum += rmsRing[i];
        return count > 0 ? sum / count : 0f;
    }

    /// <summary>HPF-Koeffizient neu berechnen (bei Runtime-Änderung des Cutoffs)</summary>
    void RecalculateHPF()
    {
        // alpha = tau / (tau + T) = 1 / (1 + 2*pi*fc/fs)
        int fs = cachedOutputSampleRate > 0
            ? cachedOutputSampleRate
            : targetSampleRate;
        hpAlpha = 1f / (1f + 2f * Mathf.PI * hpfCutoffHz / fs);
    }

    // ── Debug Gizmos ───────────────────────────────────────────────────────────

    void OnGUI()
    {
#if UNITY_EDITOR
        if (!isTalking) return;
        GUI.Label(new Rect(10, 10, 300, 20), $"RMS:  {sharedRMS:F4}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Gain: {sharedGain:F2}x");
        GUI.Label(new Rect(10, 50, 300, 20), $"Gate: {(gateOpen ? "OPEN" : "CLOSED")}");
#endif
    }
}
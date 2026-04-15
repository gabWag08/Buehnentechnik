using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class CleanMicInput : MonoBehaviour
{
    public KeyCode pushKey = KeyCode.V;

    [Header("Audio Settings")]
    public float volumeMultiplier = 1.5f;
    public float noiseThreshold = 0.0001f;
    public float smoothSpeed = 10f;

    private AudioSource audioSource;
    private string micName;
    private bool isTalking = false;

    private float[] samples = new float[128];
    private float currentVolume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            Debug.Log("Mic gefunden: " + micName);
        }
        else
        {
            Debug.LogError("❌ Kein Mikrofon gefunden!");
            return;
        }

        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // 2D Sound
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        Debug.Log("IsPlaying: " + audioSource.isPlaying);
        if (Input.GetKeyDown(pushKey) && !isTalking)
            StartTalking();

        if (Input.GetKeyUp(pushKey) && isTalking)
            StopTalking();

        if (isTalking && audioSource.isPlaying)
            ProcessAudio();
    }

    void StartTalking()
    {
        isTalking = true;
        StartCoroutine(StartMic());
    }

    IEnumerator StartMic()
{
    int minFreq, maxFreq;
    Microphone.GetDeviceCaps(micName, out minFreq, out maxFreq);
    int freq = maxFreq == 0 ? 44100 : maxFreq;

    audioSource.clip = Microphone.Start(micName, true, 10, freq);

    while (Microphone.GetPosition(micName) <= 0)
        yield return null;

    audioSource.timeSamples = Microphone.GetPosition(micName); // 🔥 WICHTIG
    audioSource.Play();

    Debug.Log("🎤 Aufnahme gestartet");
}

    void StopTalking()
    {
        isTalking = false;

        audioSource.Stop();
        Microphone.End(micName);

        Debug.Log("Aufnahme gestoppt");
    }

    void ProcessAudio()
{
    audioSource.GetOutputData(samples, 0);

    float sum = 0f;
    for (int i = 0; i < samples.Length; i++)
    {
        sum += Mathf.Abs(samples[i]);
    }

    float avg = sum / samples.Length;

    // Smooth Volume (nur Analyse)
    currentVolume = Mathf.Lerp(currentVolume, avg, Time.deltaTime * smoothSpeed);

    // 🔥 SOFT NOISE GATE
    float gate = Mathf.InverseLerp(noiseThreshold, noiseThreshold * 2f, currentVolume);

    // Smooth an/aus statt hartes muten
    audioSource.mute = gate < 0.05f;
}
}
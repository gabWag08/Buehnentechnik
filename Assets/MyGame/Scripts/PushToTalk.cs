using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CleanMicInput : MonoBehaviour
{
    public KeyCode pushKey = KeyCode.V;

    [Header("Audio Settings")]
    public float volumeMultiplier = 1.0f;
    public float noiseThreshold = 0.02f; // Noise Gate
    public float smoothSpeed = 10f;      // Glättung

    private AudioSource audioSource;
    private string micName;
    private bool isTalking = false;

    private float[] samples = new float[128];
    private float currentVolume = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            Debug.Log("Mic: " + micName);
        }
        else
        {
            Debug.LogError("Kein Mikro gefunden!");
        }

        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(pushKey) && !isTalking)
            StartTalking();

        if (Input.GetKeyUp(pushKey) && isTalking)
            StopTalking();

        if (isTalking)
            ProcessAudio();
    }

    void StartTalking()
    {
        isTalking = true;

        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(micName, out minFreq, out maxFreq);
        int freq = maxFreq == 0 ? 48000 : maxFreq;

        audioSource.clip = Microphone.Start(micName, true, 10, freq);

        while (Microphone.GetPosition(micName) <= 0) { }

        audioSource.timeSamples = Microphone.GetPosition(micName);
        audioSource.Play();
    }

    void StopTalking()
    {
        isTalking = false;
        audioSource.Stop();
        Microphone.End(micName);
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

        // Smooth Volume (wie Discord)
        currentVolume = Mathf.Lerp(currentVolume, avg, Time.deltaTime * smoothSpeed);

        // Noise Gate
        if (currentVolume < noiseThreshold)
        {
            audioSource.volume = 0f;
        }
        else
        {
            audioSource.volume = currentVolume * volumeMultiplier;
        }
    }
}
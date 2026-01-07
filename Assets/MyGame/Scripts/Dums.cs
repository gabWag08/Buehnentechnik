using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrumHit : MonoBehaviour
{
    public AudioSource audioSource;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(audioSource.clip);
        Debug.Log("Drum hit detected!");

    }
}

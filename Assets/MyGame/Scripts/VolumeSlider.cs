using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;


    public Slider volumeSlider;

    void Start()
    {
        // Anfangswert setzen
        volumeSlider.value = audioSource.volume;
        volumeSlider.value = audioSource2.volume;
        volumeSlider.value = audioSource3.volume;
        volumeSlider.value = audioSource4.volume;


        // Listener hinzufügen
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ChangeVolume(float value)
    {
        audioSource.volume = value;
        audioSource2.volume = value;
        audioSource3.volume = value;
        audioSource4.volume = value;
    }
}
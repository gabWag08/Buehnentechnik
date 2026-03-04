using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioSource audioSource;
    public Slider volumeSlider;

    void Start()
    {
        // Anfangswert setzen
        volumeSlider.value = audioSource.volume;

        // Listener hinzufügen
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    void ChangeVolume(float value)
    {
        audioSource.volume = value;
    }
}
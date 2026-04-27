using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MuicChanger : MonoBehaviour
{
    public Button button;
    public Button button2;

    public AudioClip audioClip;
    public AudioClip audioClip2;
    public VideoClip videoclip;
    public VideoClip videoclip2;

    private AudioSource[] speakers;

    public VideoPlayer videoPlayer;

    void Start()
    {
        // Alle GameObjects mit dem Tag "SPeaker" finden
        GameObject[] speakerObjects = GameObject.FindGameObjectsWithTag("Speaker");

        // AudioSources daraus holen
        speakers = new AudioSource[speakerObjects.Length];

        for (int i = 0; i < speakerObjects.Length; i++)
        {
            speakers[i] = speakerObjects[i].GetComponent<AudioSource>();
        }
    }

    public void ChangeSong1()
    {
        foreach (AudioSource source in speakers)
        {
            if (source != null)
            {
                source.clip = audioClip;
                source.Play();
            }
        }

        videoPlayer.clip = videoclip;
        videoPlayer.Play();
    }

    public void ChangeSong2()
    {
        foreach (AudioSource source in speakers)
        {
            if (source != null)
            {
                source.clip = audioClip2;
                source.Play();
            }
        }

        videoPlayer.clip = videoclip2;
        videoPlayer.Play();
    }
}
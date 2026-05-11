using UnityEngine;
using UnityEngine.Video;

public class MusicChanger : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    public VideoClip videoClip1;
    public VideoClip videoClip2;

    public VideoPlayer videoPlayer;

    private AudioSource[] speakers;

    // REFERENZ ZU DEINEM SAVE SYSTEM
    public SaveToJson saveSystem;

    void Start()
    {
        GameObject[] speakerObjects = GameObject.FindGameObjectsWithTag("Speaker");

        speakers = new AudioSource[speakerObjects.Length];

        for (int i = 0; i < speakerObjects.Length; i++)
        {
            speakers[i] = speakerObjects[i].GetComponent<AudioSource>();
        }
    }

    // =====================================================
    // SONG 1
    // =====================================================
    public void ChangeSong1()
    {
        Debug.Log("SWITCH TO SONG 1");

        SwitchAllSpeakers(audioClip1);

        if (videoPlayer != null)
        {
            videoPlayer.clip = videoClip1;
            videoPlayer.Play();
        }
    }

    // =====================================================
    // SONG 2
    // =====================================================
    public void ChangeSong2()
    {
        Debug.Log("SWITCH TO SONG 2");

        SwitchAllSpeakers(audioClip2);

        if (videoPlayer != null)
        {
            videoPlayer.clip = videoClip2;
            videoPlayer.Play();
        }
    }

    // =====================================================
    // CORE SWITCH LOGIC
    // =====================================================
    private void SwitchAllSpeakers(AudioClip newClip)
    {
        foreach (AudioSource source in speakers)
        {
            if (source == null) continue;

            // WICHTIG: alten Song sauber stoppen + loggen
            if (source.isPlaying)
            {
                if (saveSystem != null)
                {
                    saveSystem.StopSound(source);
                }

                source.Stop();
                Debug.Log("STOP OLD: " + source.clip?.name);
            }

            // neuen Song setzen
            source.clip = newClip;
            source.Play();

            Debug.Log("PLAY NEW: " + newClip.name);

            // START loggen
            if (saveSystem != null)
            {
                saveSystem.PlaySound(source);
            }
        }
    }
}
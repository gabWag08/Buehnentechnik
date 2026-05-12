using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class MusicChanger : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    [Header("Video Clips")]
    public VideoClip videoClip1;
    public VideoClip videoClip2;

    public VideoPlayer videoPlayer;

    [Header("Save System")]
    public SaveToJson saveSystem;

    // =========================
    // SONG 1
    // =========================
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

    // =========================
    // SONG 2
    // =========================
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

    // =========================
    // CORE LOGIC (SceneItem System)
    // =========================
    private void SwitchAllSpeakers(AudioClip newClip)
    {
        if (SceneItemManager.Instance == null)
        {
            Debug.LogWarning("SceneItemManager nicht gefunden!");
            return;
        }

        // Alle Items vom Typ "Speaker" holen
        List<SceneItem> speakers =
            SceneItemManager.Instance.GetItemsByType("Speaker");

        foreach (SceneItem item in speakers)
        {
            if (item == null) continue;

            AudioSource source = item.audioSource;

            if (source == null)
                continue;

            // Alten Sound stoppen
            if (source.isPlaying)
            {
                if (saveSystem != null)
                    saveSystem.StopSound(source);

                source.Stop();

                Debug.Log("STOP OLD: " + source.clip?.name);
            }

            // Neuen Clip setzen
            source.clip = newClip;
            source.Play();

            Debug.Log("PLAY NEW: " + newClip.name);

            // Save System
            if (saveSystem != null)
                saveSystem.PlaySound(source);
        }
    }
}
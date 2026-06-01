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
    if (newClip == null)
    {
        Debug.LogError("MusicChanger: NewClip ist NULL!");
        return;
    }

    if (SceneItemManager.Instance == null)
    {
        Debug.LogWarning("MusicChanger: SceneItemManager nicht gefunden!");
        return;
    }

    // Alle Speaker holen
    List<SceneItem> speakers =
        SceneItemManager.Instance.GetItemsByType("Speaker");

    Debug.Log("Speaker gefunden: " + speakers.Count);

    foreach (SceneItem item in speakers)
    {
        if (item == null)
            continue;

        AudioSource source = item.audioSource;

        if (source == null)
        {
            Debug.LogWarning(item.name + " hat keine AudioSource");
            continue;
        }

        Debug.Log("SWITCH AUDIO FOR: " + item.name);

        // Save System stoppen
        if (saveSystem != null)
            saveSystem.StopSound(source);

        // Audio sauber resetten
        source.Stop();
        source.clip = null;

        // Optional: falls 3D Audio Probleme macht
        // source.spatialBlend = 0;

        // Neuen Song setzen
        source.clip = newClip;
        source.time = 0;

        // Sicherstellen dass AudioSource aktiv ist
        source.enabled = true;
        source.gameObject.SetActive(true);

        // Starten
        source.Play();

        Debug.Log(
            "PLAY NEW: " +
            newClip.name +
            " | isPlaying=" +
            source.isPlaying
        );

        // Falls Play() auf Android nicht greift:
        if (!source.isPlaying)
        {
            Debug.LogWarning("Play fehlgeschlagen -> Retry");
            source.PlayDelayed(0.05f);
        }

        // Save System wieder aktualisieren
        if (saveSystem != null)
            saveSystem.PlaySound(source);
    }
}
}
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

    [Header("References")]
    public VideoPlayer videoPlayer;
    public SaveToJson saveSystem;

    // ==========================================
    // SONG 1
    // ==========================================
    public void ChangeSong1()
    {
        Debug.Log("=== SONG 1 BUTTON CLICKED ===");

        SwitchAllSpeakers(audioClip1);

        if (videoPlayer != null && videoClip1 != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = videoClip1;
            videoPlayer.Play();

            Debug.Log("Video 1 gestartet");
        }
    }

    // ==========================================
    // SONG 2
    // ==========================================
    public void ChangeSong2()
    {
        Debug.Log("=== SONG 2 BUTTON CLICKED ===");

        SwitchAllSpeakers(audioClip2);

        if (videoPlayer != null && videoClip2 != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = videoClip2;
            videoPlayer.Play();

            Debug.Log("Video 2 gestartet");
        }
    }

    // ==========================================
    // CORE
    // ==========================================
    private void SwitchAllSpeakers(AudioClip newClip)
    {
        Debug.Log("SwitchAllSpeakers gestartet");

        if (newClip == null)
        {
            Debug.LogError("AudioClip ist NULL!");
            return;
        }

        Debug.Log("AudioClip OK: " + newClip.name);

        if (SceneItemManager.Instance == null)
        {
            Debug.LogError("SceneItemManager.Instance ist NULL!");
            return;
        }

        Debug.Log("SceneItemManager gefunden");

        List<SceneItem> speakers =
            SceneItemManager.Instance.GetItemsByType("Speaker");

        if (speakers == null)
        {
            Debug.LogError("Speaker Liste ist NULL!");
            return;
        }

        Debug.Log("Speaker gefunden: " + speakers.Count);

        foreach (SceneItem item in speakers)
        {
            if (item == null)
            {
                Debug.LogWarning("Speaker ist NULL");
                continue;
            }

            Debug.Log("Bearbeite Speaker: " + item.name);

            AudioSource source = item.audioSource;

            if (source == null)
            {
                Debug.LogWarning(item.name + " hat keine AudioSource");

                source = item.GetComponent<AudioSource>();

                if (source == null)
                {
                    Debug.LogError(item.name + " hat wirklich keine AudioSource");
                    continue;
                }
            }

            Debug.Log("AudioSource gefunden");

            try
            {
                if (saveSystem != null)
                    saveSystem.StopSound(source);

                source.Stop();

                source.clip = newClip;

                source.enabled = true;

                if (!source.gameObject.activeSelf)
                    source.gameObject.SetActive(true);

                source.volume = 1f;

                // Nur zum Testen:
                source.spatialBlend = 0f;

                source.time = 0f;

                source.Play();

                Debug.Log(
                    "Play aufgerufen | isPlaying = " +
                    source.isPlaying
                );

                if (!source.isPlaying)
                {
                    source.PlayDelayed(0.1f);

                    Debug.Log(
                        "PlayDelayed aufgerufen | isPlaying = " +
                        source.isPlaying
                    );
                }

                if (saveSystem != null)
                    saveSystem.PlaySound(source);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    "Fehler bei Speaker " +
                    item.name +
                    "\n" +
                    ex
                );
            }
        }

        Debug.Log("SwitchAllSpeakers beendet");
    }
}
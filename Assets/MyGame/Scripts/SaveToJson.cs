using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using System.IO;

public class SaveToJson : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    // ---------------- AUDIO HISTORY ----------------
    private Dictionary<AudioSource, float> activeStartTimes = new Dictionary<AudioSource, float>();
    private List<AudioHistoryEntry> playHistory = new List<AudioHistoryEntry>();

    // =====================================================
    // AUDIO CONTROL API (WIRD VON MUSICCHANGER GENUTZT)
    // =====================================================

    public void PlaySound(AudioSource source)
    {
        if (source == null || source.clip == null) return;

        activeStartTimes[source] = Time.time;
    }

    public void StopSound(AudioSource source)
    {
        if (source == null || source.clip == null) return;

        if (!activeStartTimes.ContainsKey(source))
            return;

        float start = activeStartTimes[source];
        float end = Time.time;

        AudioHistoryEntry entry = new AudioHistoryEntry();
        entry.clipName = source.clip.name;
        entry.startTime = start;
        entry.endTime = end;

        playHistory.Add(entry);

        activeStartTimes.Remove(source);
    }

    // =====================================================
    // SAVE
    // =====================================================

    public void SaveData()
    {
        GameData data = new GameData();

        // ---------------- VIDEO ----------------
        if (videoPlayer != null)
        {
            data.videoName = videoPlayer.clip != null ? videoPlayer.clip.name : "None";
            data.videoTime = videoPlayer.time;
            data.videoIsPlaying = videoPlayer.isPlaying;
        }

        // ---------------- AUDIO STATES ----------------
        data.audioStates = new List<AudioState>();

        if (SceneItemManager.Instance != null)
        {
            List<SceneItem> speakers =
                SceneItemManager.Instance.GetItemsByType("Speaker");

            foreach (SceneItem item in speakers)
            {
                if (item == null) continue;
                if (item.audioSource == null) continue;

                AudioSource source = item.audioSource;

                AudioState state = new AudioState();

                state.clipName = source.clip != null ? source.clip.name : "None";
                state.time = source.time;
                state.isPlaying = source.isPlaying;

                state.volume = source.volume;
                state.pitch = source.pitch;
                state.panStereo = source.panStereo;
                state.spatialBlend = source.spatialBlend;

                data.audioStates.Add(state);
            }
        }

        // ---------------- HISTORY ----------------
        data.playHistory = new List<AudioHistoryEntry>(playHistory);

        // ---------------- SAVE FILE ----------------
        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/saveData.json";

        File.WriteAllText(path, json);

        Debug.Log("SAVE DONE: " + path);
    }
}

#region DATA

[System.Serializable]
public class GameData
{
    public List<AudioState> audioStates;

    public string videoName;
    public double videoTime;
    public bool videoIsPlaying;

    public List<AudioHistoryEntry> playHistory;
}

[System.Serializable]
public class AudioState
{
    public string clipName;
    public float time;
    public bool isPlaying;

    public float volume;
    public float pitch;
    public float panStereo;
    public float spatialBlend;
}

[System.Serializable]
public class AudioHistoryEntry
{
    public string clipName;
    public float startTime;
    public float endTime;
}

#endregion
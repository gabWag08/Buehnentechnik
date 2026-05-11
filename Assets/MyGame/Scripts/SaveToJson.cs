using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using System.IO;

//-------------------------------
// JSON-File wird hier gespeichert:
// C:\Users\gabri\AppData\LocalLow\DefaultCompany\Bühnentechnik
//-------------------------------

public class SaveToJson : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Slider Parent")]
    public Transform sliderParent;

    private AudioSource[] speakers;
    private Slider[] sliders;

    // ---------------- AUDIO HISTORY ----------------
    private Dictionary<AudioSource, float> activeStartTimes = new Dictionary<AudioSource, float>();
    private List<AudioHistoryEntry> playHistory = new List<AudioHistoryEntry>();

    void Start()
    {
        Debug.Log("SaveToJson START");

        GameObject[] speakerObjects = GameObject.FindGameObjectsWithTag("Speaker");

        Debug.Log("Speakers found: " + speakerObjects.Length);

        speakers = new AudioSource[speakerObjects.Length];

        for (int i = 0; i < speakerObjects.Length; i++)
        {
            speakers[i] = speakerObjects[i].GetComponent<AudioSource>();

            if (speakers[i] != null)
                Debug.Log("Loaded speaker: " + speakers[i].name);
            else
                Debug.LogWarning("Missing AudioSource on speaker " + i);
        }

        if (sliderParent != null)
        {
            sliders = sliderParent.GetComponentsInChildren<Slider>(true);
            Debug.Log("Sliders found: " + sliders.Length);
        }
        else
        {
            Debug.LogWarning("SliderParent not set!");
        }
    }

    // =====================================================
    // AUDIO CONTROL API (WIRD VON MUSICCHANGER GENUTZT)
    // =====================================================

    public void PlaySound(AudioSource source)
    {
        if (source == null || source.clip == null) return;

        Debug.Log("PlaySound: " + source.clip.name);

        activeStartTimes[source] = Time.time;
    }

    public void StopSound(AudioSource source)
    {
        if (source == null || source.clip == null) return;

        Debug.Log("StopSound: " + source.clip.name);

        if (!activeStartTimes.ContainsKey(source))
        {
            Debug.LogWarning("Stop ohne Start: " + source.clip.name);
            return;
        }

        float start = activeStartTimes[source];
        float end = Time.time;

        AudioHistoryEntry entry = new AudioHistoryEntry();
        entry.clipName = source.clip.name;
        entry.startTime = start;
        entry.endTime = end;

        playHistory.Add(entry);

        Debug.Log("HISTORY ADDED: " + entry.clipName +
                  " | " + start + " → " + end +
                  " (" + (end - start) + "s)");

        activeStartTimes.Remove(source);
    }

    // =====================================================
    // SAVE
    // =====================================================

    public void SaveData()
    {
        Debug.Log("SAVE START");

        GameData data = new GameData();

        // ---------------- VIDEO ----------------
        if (videoPlayer != null)
        {
            data.videoName = videoPlayer.clip != null ? videoPlayer.clip.name : "None";
            data.videoTime = videoPlayer.time;
            data.videoIsPlaying = videoPlayer.isPlaying;

            Debug.Log("🎬 Video saved: " + data.videoName + " | time: " + data.videoTime);
        }

        // ---------------- AUDIO STATES ----------------
        data.audioStates = new List<AudioState>();

        if (speakers != null)
        {
            foreach (AudioSource source in speakers)
            {
                if (source == null) continue;

                AudioState state = new AudioState();

                state.clipName = source.clip != null ? source.clip.name : "None";
                state.time = source.time;
                state.isPlaying = source.isPlaying;

                state.volume = source.volume;
                state.pitch = source.pitch;
                state.panStereo = source.panStereo;
                state.spatialBlend = source.spatialBlend;

                data.audioStates.Add(state);

                Debug.Log("State saved: " + state.clipName +
                          " | playing: " + state.isPlaying +
                          " | time: " + state.time);
            }
        }

        // ---------------- SLIDERS ----------------
        data.sliderValues = new List<float>();

        if (sliders != null)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                data.sliderValues.Add(sliders[i].value);
                Debug.Log("Slider " + i + ": " + sliders[i].value);
            }
        }

        // ---------------- HISTORY ----------------
        data.playHistory = new List<AudioHistoryEntry>(playHistory);

        Debug.Log("History count: " + data.playHistory.Count);

        foreach (var h in data.playHistory)
        {
            Debug.Log("ENTRY: " + h.clipName +
                      " | " + h.startTime +
                      " → " + h.endTime);
        }

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

    public List<float> sliderValues;

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
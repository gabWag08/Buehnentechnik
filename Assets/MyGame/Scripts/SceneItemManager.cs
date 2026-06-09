using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneItemManager : MonoBehaviour
{
    public static SceneItemManager Instance;

    private Dictionary<string, List<SceneItem>> items = new Dictionary<string, List<SceneItem>>();

    [Header("UI")]
    public GameObject sliderPrefab;
    public Transform sliderParent;

    private Dictionary<SceneItem, GameObject> itemUI = new Dictionary<SceneItem, GameObject>();

    // =========================
    // START (UNVERÄNDERT)
    // =========================

    private void Start()
    {
        Instance = this;

        var sceneItems = FindObjectsByType<SceneItem>(
            FindObjectsSortMode.None);

        foreach (var item in sceneItems)
        {
            Register(item);
        }
    }

    // =========================
    // REGISTRATION
    // =========================

    public void Register(SceneItem item)
    {
        if (item == null) return;

        if (!items.ContainsKey(item.itemType))
            items[item.itemType] = new List<SceneItem>();

        if (!items[item.itemType].Contains(item))
            items[item.itemType].Add(item);

        CreateUI(item);
    }

    public void Unregister(SceneItem item)
    {
        if (item == null) return;

        if (items.ContainsKey(item.itemType))
        {
            items[item.itemType].Remove(item);
        }

        RemoveUI(item);
    }

    public void DeleteAll()
    {
        foreach (var list in items.Values)
        {
            foreach (var item in list)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }

        items.Clear();

        foreach (var ui in itemUI.Values)
        {
            if (ui != null)
                Destroy(ui);
        }

        itemUI.Clear();
    }

    public List<SceneItem> GetItemsByType(string type)
    {
        if (items.ContainsKey(type))
            return items[type];

        return new List<SceneItem>();
    }

    public Dictionary<string, List<SceneItem>> GetAllItems()
    {
        return items;
    }

    // =========================
    // UI
    // =========================

    void CreateUI(SceneItem item)
    {
        if (item.audioSource == null)
        {
            Debug.LogWarning("SceneItem hat keine AudioSource: " + item.displayName);
            return;
        }

        if (itemUI.ContainsKey(item))
            return; // verhindert doppelte UI

        GameObject uiObj = Instantiate(sliderPrefab, sliderParent);
        itemUI[item] = uiObj;

        Slider[] sliders = uiObj.GetComponentsInChildren<Slider>();

        Slider volumeSlider = null;
        Slider pitchSlider = null;
        Slider stereoPanSlider = null;

        foreach (var s in sliders)
        {
            string n = s.name.ToLower();

            if (n.Contains("volume"))
                volumeSlider = s;

            if (n.Contains("pitch"))
                pitchSlider = s;

            if (n.Contains("stereopan"))
                stereoPanSlider = s;
        }

        TMP_Text text = uiObj.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = item.displayName + " (" + item.itemType + ")";
        }

        AudioSource audio = item.audioSource;

        // =========================
        // VOLUME
        // =========================
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = audio.volume;

            volumeSlider.onValueChanged.AddListener((value) =>
            {
                if (audio != null)
                    audio.volume = value;
            });
        }

        // =========================
        // PITCH
        // =========================
        if (pitchSlider != null)
        {
            pitchSlider.minValue = 0.5f;
            pitchSlider.maxValue = 2f;
            pitchSlider.value = audio.pitch;

            pitchSlider.onValueChanged.AddListener((value) =>
            {
                if (audio != null)
                    audio.pitch = value;
            });
        }

        // =========================
        // STEREO PAN
        // =========================
        if (stereoPanSlider != null)
        {
            stereoPanSlider.minValue = -1f;
            stereoPanSlider.maxValue = 1f;
            stereoPanSlider.value = audio.panStereo;

            stereoPanSlider.onValueChanged.AddListener((value) =>
            {
                if (audio != null)
                    audio.panStereo = value;
            });
        }
    }

    void RemoveUI(SceneItem item)
    {
        if (itemUI.ContainsKey(item))
        {
            if (itemUI[item] != null)
                Destroy(itemUI[item]);

            itemUI.Remove(item);
        }
    }
}
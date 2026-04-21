using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Falls du TextMeshPro benutzt

public class SceneItemManager : MonoBehaviour
{
    public static SceneItemManager Instance;

    private Dictionary<string, List<SceneItem>> items = new Dictionary<string, List<SceneItem>>();

    [Header("UI")]
    public GameObject sliderPrefab;
    public Transform sliderParent;

    private Dictionary<SceneItem, GameObject> itemUI = new Dictionary<SceneItem, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(SceneItem item)
    {
        if (!items.ContainsKey(item.itemType))
            items[item.itemType] = new List<SceneItem>();

        items[item.itemType].Add(item);

        CreateUI(item);
    }

    public void Unregister(SceneItem item)
    {
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
        GameObject uiObj = Instantiate(sliderPrefab, sliderParent);
        itemUI[item] = uiObj;

        Slider slider = uiObj.GetComponentInChildren<Slider>();

        // Text setzen (Name anzeigen)
        TMP_Text text = uiObj.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = item.displayName + " (" + item.itemType + ")";
        }

        // Beispielwerte
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 0.5f;

            slider.onValueChanged.AddListener((value) =>
            {
                Debug.Log(item.displayName + " Slider: " + value);
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
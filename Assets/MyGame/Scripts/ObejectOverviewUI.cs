using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectOverviewUI : MonoBehaviour
{
    public Transform overviewParent;
    public Transform detailParent;

    public GameObject overviewEntryPrefab;
    public GameObject detailEntryPrefab;

    // =========================
    // OVERVIEW
    // =========================
    public void RefreshOverview()
    {
        if (overviewParent == null || overviewEntryPrefab == null)
        {
            Debug.LogError("Overview references not assigned!");
            return;
        }

        Clear(overviewParent);

        if (SceneItemManager.Instance == null)
        {
            Debug.LogError("SceneItemManager is NULL!");
            return;
        }

        var allItems = SceneItemManager.Instance.GetAllItems();

        foreach (var pair in allItems)
        {
            string type = pair.Key;
            int count = pair.Value.Count;

            GameObject entry = Instantiate(overviewEntryPrefab, overviewParent);

            // Set Text
            var text = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{type} : {count}";
            }
            else
            {
                Debug.LogError("No TMP Text found in OverviewEntryPrefab!");
            }

            // Button
            var btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => ShowDetails(type));
            }
            else
            {
                Debug.LogWarning("No Button found on OverviewEntryPrefab!");
            }
        }
    }

    // =========================
    // DELAYED REFRESH (UI FIX)
    // =========================
    public void RefreshOverviewDelayed()
    {
        StartCoroutine(RefreshNextFrame());
    }

    private System.Collections.IEnumerator RefreshNextFrame()
    {
        yield return null;
        RefreshOverview();
    }

    // =========================
    // DETAILS
    // =========================
    void ShowDetails(string type)
    {
        if (detailParent == null || detailEntryPrefab == null)
        {
            Debug.LogError("Detail references not assigned!");
            return;
        }

        Clear(detailParent);

        if (SceneItemManager.Instance == null)
        {
            Debug.LogError("SceneItemManager is NULL!");
            return;
        }

        List<SceneItem> items = SceneItemManager.Instance.GetItemsByType(type);

        int index = 1;

        foreach (var item in items)
        {
            if (item == null) continue;

            GameObject entry = Instantiate(detailEntryPrefab, detailParent);

            // Set Text
            var text = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{type}{index}";
            }
            else
            {
                Debug.LogError("No TMP Text found in DetailEntryPrefab!");
            }

            // Button (Highlight)
            var btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(Blink(item));
                });
            }

            index++;
        }
    }

    // =========================
    // BLINK / HIGHLIGHT
    // =========================
    System.Collections.IEnumerator Blink(SceneItem item)
    {
        if (item == null) yield break;

        Renderer rend = item.GetComponentInChildren<Renderer>();
        if (rend == null) yield break;

        Material mat = rend.material;
        Color original = mat.color;

        for (int i = 0; i < 6; i++)
        {
            mat.color = Color.yellow;
            yield return new WaitForSeconds(0.25f);

            mat.color = original;
            yield return new WaitForSeconds(0.25f);
        }
    }

    // =========================
    // UTIL
    // =========================
    void Clear(Transform parent)
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeleteAll()
    {
        if (SceneItemManager.Instance == null)
        {
            Debug.LogError("SceneItemManager is NULL!");
            return;
        }

        SceneItemManager.Instance.DeleteAll();

        RefreshOverview();
        Clear(detailParent);
    }
}

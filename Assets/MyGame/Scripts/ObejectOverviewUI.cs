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

    public void RefreshOverview()
    {
        Clear(overviewParent);

        var allItems = SceneItemManager.Instance.GetAllItems();

        foreach (var pair in allItems)
        {
            string type = pair.Key;
            int count = pair.Value.Count;

            GameObject entry = Instantiate(overviewEntryPrefab, overviewParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"{type} : {count}";

            Button btn = entry.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowDetails(type));
        }
    }

    private System.Collections.IEnumerator RefreshNextFrame()
    {
        yield return null; // wait 1 frame
        RefreshOverview();
    }

    public void RefreshOverviewDelayed()
    {
        StartCoroutine(RefreshNextFrame());
    }


    void ShowDetails(string type)
    {
        Clear(detailParent);

        List<SceneItem> items = SceneItemManager.Instance.GetItemsByType(type);

        int index = 1;

        foreach (var item in items)
        {
            GameObject entry = Instantiate(detailEntryPrefab, detailParent);

            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"{type}{index}";

            // Select / highlight
            entry.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(Blink(item));
            });

            // Delete button (assumes child button exists)
            Button deleteBtn = entry.transform.Find("DeleteButton").GetComponent<Button>();
            deleteBtn.onClick.AddListener(() =>
            {
                Destroy(item.gameObject);
                RefreshOverview();
                ShowDetails(type);
            });

            index++;
        }
    }

    System.Collections.IEnumerator Blink(SceneItem item)
    {
        Renderer rend = item.GetComponentInChildren<Renderer>();
        if (rend == null) yield break;

        Color original = rend.material.color;

        for (int i = 0; i < 6; i++)
        {
            rend.material.color = Color.yellow;
            yield return new WaitForSeconds(0.25f);
            rend.material.color = original;
            yield return new WaitForSeconds(0.25f);
        }
    }

    void Clear(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeleteAll()
    {
        SceneItemManager.Instance.DeleteAll();
        RefreshOverview();
        Clear(detailParent);
    }
}

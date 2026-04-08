using System.Collections.Generic;
using UnityEngine;

public class SceneItemManager : MonoBehaviour
{
    public static SceneItemManager Instance;

    private Dictionary<string, List<SceneItem>> items = new Dictionary<string, List<SceneItem>>();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(SceneItem item)
    {
        if (!items.ContainsKey(item.itemType))
            items[item.itemType] = new List<SceneItem>();

        items[item.itemType].Add(item);
    }

    public void Unregister(SceneItem item)
    {
        if (items.ContainsKey(item.itemType))
        {
            items[item.itemType].Remove(item);
        }
    }

    public Dictionary<string, List<SceneItem>> GetAllItems()
    {
        return items;
    }

    public List<SceneItem> GetItemsByType(string type)
    {
        if (items.ContainsKey(type))
            return items[type];

        return new List<SceneItem>();
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
    }
}

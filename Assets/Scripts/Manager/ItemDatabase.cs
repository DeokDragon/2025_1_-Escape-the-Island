using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    [SerializeField]
    private List<Item> items = new List<Item>();
    private Dictionary<string, Item> itemDict = new Dictionary<string, Item>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            var loadedItems = Resources.LoadAll<Item>("ItemData");
            foreach (var item in loadedItems)
            {
                if (!items.Contains(item))
                    items.Add(item);
            }

            foreach (var item in items)
            {
                string key = item.itemName.Trim();
                if (!itemDict.ContainsKey(key))
                    itemDict.Add(key, item);
            }

            Debug.Log($"[ItemDatabase] 총 아이템 수: {items.Count}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetItemByName(string name)
    {
        name = name.Trim();
        if (itemDict.TryGetValue(name, out var item))
            return item;

        Debug.LogWarning("[ItemDatabase] 아이템 못 찾음: " + name);
        return null;
    }
}
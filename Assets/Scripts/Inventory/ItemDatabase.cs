using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    [SerializeField]
    private List<Item> items;

    private void Awake()
    {
        instance = this;
    }

    public Item GetItemByName(string name)
    {
        return items.Find(item => item.itemName == name);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    [SerializeField]
    private List<Item> items;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 유지됨
        }
        else
        {
            Destroy(gameObject);  // 중복 방지
        }
    }


    public Item GetItemByName(string name)
    {
        return items.Find(item => item.itemName == name);
    }
}

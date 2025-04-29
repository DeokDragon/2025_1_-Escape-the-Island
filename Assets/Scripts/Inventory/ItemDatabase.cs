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
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �� ������
        }
        else
        {
            Destroy(gameObject);  // �ߺ� ����
        }
    }


    public Item GetItemByName(string name)
    {
        return items.Find(item => item.itemName == name);
    }
}

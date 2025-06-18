using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.IO;

public class ItemDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "item_export.json";  // 확장자 포함, StreamingAssets 폴더에 위치

    private List<ItemData> itemDataList;

    void Start()
    {
        LoadItemData();
    }

    void LoadItemData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            ItemDataListWrapper wrapper = JsonUtility.FromJson<ItemDataListWrapper>(jsonText);
            itemDataList = wrapper.items;

            Debug.Log($"로드된 아이템 수: {itemDataList.Count}");

            foreach (var item in itemDataList)
            {
                Debug.Log($"- 아이템: {item.itemName}, 설명: {item.itemDesc}");
            }
        }
        else
        {
            Debug.LogError($"❌ JSON 파일을 찾을 수 없습니다: {path}");
        }
    }
}

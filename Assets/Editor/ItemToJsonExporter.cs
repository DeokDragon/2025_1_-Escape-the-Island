#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ItemToJsonExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Items to JSON")]
    public static void ExportItemsToJson()
    {
        Item[] items = Resources.LoadAll<Item>("Item");

        List<ItemData> itemDataList = new List<ItemData>();
        foreach (var item in items)
        {
            itemDataList.Add(ConvertToData(item));
        }

        ItemDataListWrapper wrapper = new ItemDataListWrapper { items = itemDataList };
        string json = JsonUtility.ToJson(wrapper, true);

        string directory = Path.Combine(Application.dataPath, "StreamingAssets");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string path = Path.Combine(Application.streamingAssetsPath, "item_export.json");
        File.WriteAllText(path, json);

        Debug.Log($"✅ Item data exported to: {path}");
        AssetDatabase.Refresh();
    }

    private static ItemData ConvertToData(Item item)
    {
        return new ItemData
        {
            itemName = item.itemName,
            itemDesc = item.itemDesc,
            itemType = item.itemType.ToString(),
            itemImagePath = item.itemImage != null ? AssetDatabase.GetAssetPath(item.itemImage) : "",
            itemPrefabPath = item.itemPrefab != null ? AssetDatabase.GetAssetPath(item.itemPrefab) : "",
            weaponType = item.weaponType,
            attackPower = item.attackPower,
            materialType = item.materialType.ToString(),
            maxStackCount = item.maxStackCount
        };
    }
}
#endif
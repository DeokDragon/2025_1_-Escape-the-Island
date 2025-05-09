using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;

public class ItemImporter : EditorWindow
{
    private TextAsset jsonFile; // 드래그 앤 드롭 JSON
    private string savePath = "Assets/Resources/Item/";

    [MenuItem("Tools/Import Items from JSON")]
    public static void ShowWindow()
    {
        GetWindow<ItemImporter>("Item Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Items from JSON", EditorStyles.boldLabel);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

        if (GUILayout.Button("Import"))
        {
            if (jsonFile == null)
            {
                Debug.LogError("JSON 파일을 선택하세요.");
                return;
            }

            ImportItems(jsonFile.text);
        }
    }

    [System.Serializable]
    public class ItemData
    {
        public int id;
        public string itemName;
        public string itemDesc;
        public string itemType;
        public string itemImage;  
        public string itemPrefab;  
        public string waponType;   
    }

    private void ImportItems(string json)
    {
        List<ItemData> itemList;
        try
        {
            itemList = JsonConvert.DeserializeObject<List<ItemData>>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON 파싱 실패: " + e.Message);
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        foreach (var data in itemList)
        {
            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.itemName = data.itemName;
            newItem.itemDesc = data.itemDesc;

            if (System.Enum.TryParse(data.itemType, out Item.ItemType parsedType))
                newItem.itemType = parsedType;
            else
                Debug.LogWarning($"Unknown itemType: {data.itemType}");

            newItem.weaponType = data.waponType;

            // 아이콘 로드
            string iconPath = $"Assets/UI/Icons/Icon/{data.itemImage}.png";
            newItem.itemImage = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (newItem.itemImage == null)
                Debug.LogWarning($"❌ UI 이미지가 존재하지 않음: {iconPath}");

            // 프리팹 로드 (Assets/3DModel/3DModel + 하위 폴더 전체에서 이름 기준으로 검색)
            newItem.itemPrefab = LoadPrefabFromAssetsByName(data.itemPrefab);
            if (newItem.itemPrefab == null)
                Debug.LogWarning($"❌ 프리팹이 'Assets/3DModel/ModelPrefabs' 하위 폴더에서 로드되지 않음: {data.itemPrefab}");

            // ScriptableObject 저장
            string assetPath = Path.Combine(savePath, $"{data.itemName}_{data.id}.asset");
            AssetDatabase.CreateAsset(newItem, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ 아이템 임포트 완료! 저장 경로: " + savePath);
    }

    //'Assets/3DModel/3DModel' 폴더 및 모든 하위 폴더에서 프리팹 이름으로 검색
    private GameObject LoadPrefabFromAssetsByName(string prefabName)
    {
        string[] guids = AssetDatabase.FindAssets($"{prefabName} t:prefab", new[] { "Assets/3DModel/ModelPrefabs" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.name == prefabName)
            {
                return prefab;
            }
        }

        Debug.LogWarning($"❌ 'Assets/3DModel/ModelPrefabs' 하위에서 이름이 '{prefabName}'인 프리팹을 찾을 수 없음");
        return null;
    }
}
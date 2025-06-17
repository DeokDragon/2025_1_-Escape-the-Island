using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public SaveData CurrentSaveData { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeNewGame()
    {
        CurrentSaveData = new SaveData();
    }

    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/save{slotIndex}.json";
    }

    public void SaveToSlot(int slotIndex)
    {
        Debug.Log($"💾 저장 시작: 슬롯 {slotIndex}");

        SaveData data = CurrentSaveData ?? new SaveData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            data.playerPosition = player.transform.position;

        StatusController status = player?.GetComponent<StatusController>();
        if (status != null)
        {
            data.hp = status.GetCurrentHP();
            data.stamina = status.GetCurrentStamina();
            data.hunger = status.GetCurrentHunger();
            data.thirst = status.GetCurrentThirst();
        }

        Inventory inventory = Inventory.instance;
        if (inventory != null)
        {
            var slots = inventory.GetSlots();
            data.inventorySlots.Clear();
            foreach (var slot in slots)
            {
                if (slot.item != null)
                    data.inventorySlots.Add(new InventorySlotData { itemName = slot.item.itemName, itemCount = slot.itemCount });
            }
        }

        QuickSlotController quickSlot = FindObjectOfType<QuickSlotController>();
        if (quickSlot != null)
        {
            Slot[] slots = quickSlot.GetQuickSlots();
            data.quickSlotDataList.Clear();
            foreach (Slot slot in slots)
            {
                if (slot.item != null)
                    data.quickSlotDataList.Add(new QuickSlotData { itemName = slot.item.itemName, itemCount = slot.itemCount });
                else
                    data.quickSlotDataList.Add(new QuickSlotData { itemName = "", itemCount = 0 });
            }
        }

        CaveRandomizer caveRandomizer = FindObjectOfType<CaveRandomizer>();
        if (caveRandomizer != null)
        {
            for (int i = 0; i < caveRandomizer.caveSpawns.Length; i++)
            {
                if (caveRandomizer.caveSpawns[i].activeSelf)
                {
                    data.caveIndex = i;
                    break;
                }
            }
        }

        DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
        if (timeSystem != null)
            data.currentTime = timeSystem.currentTime;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slotIndex), json);
        CurrentSaveData = data;
    }

    public SaveData LoadFromSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var objData in data.spawnedObjects)
        {
            GameObject prefab = Resources.Load<GameObject>($"InstallablePrefabs/{objData.prefabName}");
            if (prefab != null)
                Instantiate(prefab, objData.position, Quaternion.Euler(objData.rotation));
            else
                Debug.LogWarning($"[SaveManager] 프리팹 {objData.prefabName} 로드 실패");
        }

        DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
        if (timeSystem != null)
            timeSystem.SetTime(data.currentTime);

        CurrentSaveData = data;
        return data;
    }

    public void DeleteSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (File.Exists(path))
            File.Delete(path);
    }

    public bool HasSaveFile(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        return File.Exists(path);
    }

}

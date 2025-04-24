using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // 씬 이동해도 유지됨
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/save{slotIndex}.json";
    }

    public void SaveToSlot(int slotIndex)
    {
       

        SaveData data = new SaveData();

        // 1. 플레이어 위치 저장
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;
            
        }

        // 2. 상태 저장
        StatusController status = player?.GetComponent<StatusController>();
        if (status != null)
        {
            data.hp = status.GetCurrentHP();
            data.stamina = status.GetCurrentStamina();
            data.hunger = status.GetCurrentHunger();
            data.thirst = status.GetCurrentThirst();
            
        }

        // 3. 인벤토리 저장
        Inventory inventory = Inventory.instance;
        if (inventory != null)
        {
            var slots = inventory.GetSlots();
            
            foreach (var slot in slots)
            {
                if (slot.item != null)
                {
                    InventorySlotData slotData = new InventorySlotData
                    {
                        itemName = slot.item.itemName,
                        itemCount = slot.itemCount
                    };
                    data.inventorySlots.Add(slotData);
                    
                }
            }
        }

        // 4. 퀵슬롯 저장 (✅ 저장 파일 생성 전에)
        QuickSlotController quickSlot = FindObjectOfType<QuickSlotController>();
        if (quickSlot != null)
        {
            Slot[] slots = quickSlot.GetQuickSlots();
            foreach (Slot slot in slots)
            {
                if (slot.item != null)
                {
                    QuickSlotData slotData = new QuickSlotData
                    {
                        itemName = slot.item.itemName,
                        itemCount = slot.itemCount
                    };
                    data.quickSlotDataList.Add(slotData);
                }
                else
                {
                    data.quickSlotDataList.Add(new QuickSlotData { itemName = "", itemCount = 0 });
                }
            }
        }

        // 5. JSON 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slotIndex), json);
        
    }

    public SaveData LoadFromSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            

            return data;
        }
        else
        {
           
            return null;
        }
    }

    public bool HasSaveFile(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        return File.Exists(path);
    }
}

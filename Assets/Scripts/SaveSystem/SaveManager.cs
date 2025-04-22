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
        Debug.Log("💾 SaveToSlot 시작됨");

        SaveData data = new SaveData();

        // 🔍 1. 플레이어 위치 저장
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ Player 오브젝트를 찾지 못함! (태그 확인 필요)");
        }
        else
        {
            data.playerPosition = player.transform.position;
            Debug.Log("✅ 위치 저장됨: " + data.playerPosition);
        }

        // 🔍 2. 상태 저장
        StatusController status = player?.GetComponent<StatusController>();
        if (status == null)
        {
            Debug.LogError("❌ StatusController 컴포넌트를 찾지 못함!");
        }
        else
        {
            data.hp = status.GetCurrentHP();
            data.stamina = status.GetCurrentStamina();
            data.hunger = status.GetCurrentHunger();
            data.thirst = status.GetCurrentThirst();
            Debug.Log("✅ 상태 저장됨: HP=" + data.hp + " SP=" + data.stamina);
        }

        // 🔍 3. 인벤토리 저장
        Inventory inventory = Inventory.instance;
        if (inventory == null)
        {
            Debug.LogError("❌ Inventory.instance가 null임!");
        }
        else
        {
            var slots = inventory.GetSlots();
            Debug.Log("✅ 슬롯 수: " + slots.Length);
            foreach (var slot in slots)
            {
                if (slot.item != null)
                {
                    InventorySlotData slotData = new InventorySlotData();
                    slotData.itemName = slot.item.itemName;
                    slotData.itemCount = slot.itemCount;
                    data.inventorySlots.Add(slotData);
                    Debug.Log("📦 아이템 저장됨: " + slotData.itemName + " x" + slotData.itemCount);
                }
            }
        }

        // 🔍 4. 파일 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slotIndex), json);
        Debug.Log("✅ 파일 저장 완료! 경로: " + GetSaveFilePath(slotIndex));
    }


    public SaveData LoadFromSlot(int slotIndex)
       {
        string path = GetSaveFilePath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"슬롯 {slotIndex + 1} 로드 완료!");
            return data;
        }
        else
        {
            Debug.LogWarning($"슬롯 {slotIndex + 1}에 저장된 파일이 없습니다.");
            return null;
        }
    }

    public bool HasSaveFile(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        return File.Exists(path);
    }

}





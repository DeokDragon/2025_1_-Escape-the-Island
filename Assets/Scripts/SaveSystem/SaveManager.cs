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

    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/save{slotIndex}.json";
    }

    public void SaveToSlot(int slotIndex)
    {
        Debug.Log($"💾 저장 시작: 슬롯 {slotIndex}");

        SaveData data = CurrentSaveData ?? new SaveData();

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

        // 4. 퀵슬롯 저장
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

        // 5. 동굴 위치 저장
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

        // ✅ 6. 시간 저장 (여기 완전히 고침!)
        DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
        if (timeSystem != null)
        {
            data.currentTime = timeSystem.currentTime;  // 기존 transform.eulerAngles.x → 정확히 currentTime으로 수정
        }

        // ✅ 7. 설치 오브젝트 저장 (이미 spawnedObjects에 들어가있음 → 건드릴 필요 없음)

        // 8. 실제 JSON으로 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slotIndex), json);

        // ✅ 저장 후 현재 세이브데이터도 갱신
        CurrentSaveData = data;
    }

    public SaveData LoadFromSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // ✅ 설치된 오브젝트 복구
            foreach (var objData in data.spawnedObjects)
            {
                Debug.Log($"프리팹 로드 시도: {objData.prefabName}");

                GameObject prefab = Resources.Load<GameObject>($"InstallablePrefabs/{objData.prefabName}");
                if (prefab != null)
                {
                    Debug.Log($"성공적으로 프리팹 로드됨: {objData.prefabName}");
                    Instantiate(prefab, objData.position, Quaternion.Euler(objData.rotation));
                }
                else
                {
                    Debug.LogWarning($"프리팹 로드 실패: {objData.prefabName}");
                }
            }

            // ✅ 시간 복구 추가
            DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
            if (timeSystem != null)
            {
                timeSystem.SetTime(data.currentTime);
            }

            // ✅ 로드시 CurrentSaveData 갱신
            CurrentSaveData = data;

            return data;
        }
        else
        {
            Debug.LogWarning($"Save file not found: {path}");
            return null;
        }
    }

    public bool HasSaveFile(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        return File.Exists(path);
    }

    public void DeleteSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.Log($" 슬롯 {slotIndex}에는 삭제할 파일이 없음.");
        }
    }
}

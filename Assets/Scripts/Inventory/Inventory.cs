using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    // ✅ 싱글톤 인스턴스 추가
    public static Inventory instance;

    private void Awake()
    {
        // 인스턴스가 없으면 할당, 있으면 중복 방지
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    public static bool inventoryActivated = false;

    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;

    // 슬롯들
    private Slot[] slots;

    void Start()
    {
        
    }

    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        Debug.Log("AcquireItem called: " + _item.itemName + " x" + _count);

        // 같은 아이템이 인벤토리에 있는지 먼저 확인
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemName == _item.itemName)
            {
                slots[i].SetSlotCount(_count);  // 아이템 수량만 증가
                slots[i].UpdateSlotUI();        // UI 업데이트
                return;
            }
        }

        // 같은 아이템이 없을 경우, 새로운 슬롯에 추가
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                slots[i].UpdateSlotUI();        // UI 업데이트
                return;
            }
        }
    }

    // 아이템을 가지고 있는지 확인
    public bool HasItem(string itemName, int count)
    {
        foreach (Slot slot in slots)
        {
            if (slot.item != null)
            {
                Debug.Log("아이템 이름 확인: " + slot.item.itemName);  // 아이템 이름 확인
                if (slot.item.itemName.ToLower() == itemName.ToLower())  // 대소문자 구분 없이 비교
                {
                    Debug.Log("아이템 수량: " + slot.itemCount);  // 수량 확인
                    if (slot.itemCount >= count)
                        return true;
                }
            }
        }
        return false;
    }

    // 아이템 소비하기
    public bool ConsumeItem(string itemName, int count)
    {
        Debug.Log($"ConsumeItem 호출됨: {itemName} x{count}");

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemName == itemName)
            {
                if (slots[i].itemCount >= count)
                {
                    Debug.Log($"아이템 소비 전: {slots[i].itemCount}");
                    slots[i].SetSlotCount(-count);  // 아이템 개수 감소
                    slots[i].UpdateSlotUI();        // UI 업데이트
                    Debug.Log($"아이템 소비 후: {slots[i].itemCount}");
                    return true;
                }
            }
        }

        return false;  // 아이템을 충분히 소모할 수 없으면 false 반환
    }

    // 특정 아이템의 개수를 반환하는 함수
    public int GetItemCount(string itemName)
    {
        int itemCount = 0;

        // 모든 슬롯을 확인해서 해당 아이템의 개수를 합산
        foreach (Slot slot in slots) // items 대신 slots 사용
        {
            if (slot.item != null && slot.item.itemName == itemName)
            {
                itemCount += slot.itemCount; // 아이템의 개수를 더함
            }
        }

        return itemCount; // 총 아이템 개수 반환
    }

    public Slot[] GetSlots()
    {
        return slots;
    }

    public void LoadInventory(List<InventorySlotData> slotDataList)
    {
        // 모든 슬롯 비우기
        foreach (Slot slot in slots)    
        {
            slot.ClearSlot(); // 이 함수가 없으면 AddItem 전에 null 초기화 코드 직접 넣어야 해
        }

        // 전달받은 데이터로 슬롯 채우기
        for (int i = 0; i < slotDataList.Count && i < slots.Length; i++)
        {
            InventorySlotData data = slotDataList[i];
            if (!string.IsNullOrEmpty(data.itemName))
            {
                Item item = ItemDatabase.instance.GetItemByName(data.itemName); // 아이템 이름으로 찾기
                if (item != null)
                {
                    slots[i].AddItem(item, data.itemCount);
                    slots[i].UpdateSlotUI(); // UI 갱신 함수
                }
            }
        }

        Debug.Log("인벤토리 로드 완료!");
    }

    

}
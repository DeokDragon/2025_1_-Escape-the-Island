using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    //인스턴스
    public static Inventory instance;
    public ItemDatabase itemDatabase;
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
    [SerializeField]
    private GameObject go_QuickSlotParent;


    //사운드 설정
    [SerializeField] private AudioSource audioSource;   // 사운드 재생용 AudioSource
    [SerializeField] private AudioClip OpenSound; //사운드


    // 슬롯들
    private Slot[] slots;
    private Slot[] quickSlots;
    //넣었나 안넣었나?
    private bool isNotPut;

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickSlots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();
    }
    //인벤토리 열기 시도
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyBindingManager.GetKey("Inventory", KeyCode.I)))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }
    //인벤토리 열기
    private void OpenInventory()
    {
        PlayOpenSound();
        go_InventoryBase.SetActive(true);

        GameManager.isOpenInventory = true;
        GameManager.UpdateCursorState();
    }

    //인벤토리 닫음
    private void CloseInventory()
    {
        PlayOpenSound();
        go_InventoryBase.SetActive(false);

        GameManager.isOpenInventory = false;
        GameManager.UpdateCursorState();
    }
    // 아이템 횔득
    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickSlots, _item, _count);
        if (isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("퀵슬롯과 인벤토리가 꽉찼습니다");
    }

    private void PutSlot(Slot[] _slots, Item _item, int _count)

    {// 같은 아이템이 인벤토리에 있는지 먼저 확인
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null && _slots[i].item.itemName == _item.itemName)
                {
                    _slots[i].SetSlotCount(_count);  // 아이템 수량만 증가
                    isNotPut = false;
                    _slots[i].UpdateSlotUI();        
                    return;
                }
            }
        }

        // 같은 아이템이 없을 경우, 새로운 슬롯에 추가
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item == null)
            {
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                _slots[i].UpdateSlotUI(); 
                return;
            }
        }
        isNotPut = true;

    }

    // 아이템을 가지고 있는지 확인
    public bool HasItem(string itemName, int count)
    {
        int totalCount = 0;

        foreach (Slot slot in slots.Concat(quickSlots)) // 퀵슬롯도 포함
        {
            if (slot.item != null && slot.item.itemName.ToLower() == itemName.ToLower())
            {
                totalCount += slot.itemCount;
                if (totalCount >= count)
                    return true;
            }
        }
        return false;
    }

    // 아이템 소비하기
    public bool ConsumeItem(string itemName, int count)
    {
        int remaining = count;

        foreach (Slot slot in slots.Concat(quickSlots))
        {
            if (slot.item != null && slot.item.itemName == itemName)
            {
                if (slot.itemCount >= remaining)
                {
                    slot.SetSlotCount(-remaining);
                    slot.UpdateSlotUI();
                    return true;
                }
                else
                {
                    remaining -= slot.itemCount;
                    slot.SetSlotCount(-slot.itemCount); // 남은 걸 다 소모
                    slot.UpdateSlotUI();
                }
            }
        }

        return false;
    }

    // 특정 아이템의 개수를 반환하는 함수
    public int GetItemCount(string itemName)
    {
        int itemCount = 0;
        foreach (Slot slot in slots.Concat(quickSlots))
        {
            if (slot.item != null && slot.item.itemName == itemName)
            {
                itemCount += slot.itemCount;
            }
        }
        return itemCount;
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
            slot.ClearSlot(); 
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
                    slots[i].UpdateSlotUI(); 
                }
            }
        }
    }

    public void AcquireItemByName(string itemName, int count = 1)
    {
        Item item = itemDatabase.GetItemByName(itemName); // 이름으로 Item 찾기
        if (item != null)
        {
            AcquireItem(item, count);
        }
        else
        {
            Debug.LogWarning($"[Inventory] '{itemName}' 아이템을 찾을 수 없습니다.");
        }
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && OpenSound != null)
            audioSource.PlayOneShot(OpenSound);
    }


}
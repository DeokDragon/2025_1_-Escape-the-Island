using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Inventory : MonoBehaviour
{

    public static bool inventoryActivated = false;

    //필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;



    //슬롯들
    private Slot[] slots;
    // Start is called before the first frame update
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    // Update is called once per frame
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

        // ✅ 같은 아이템이 인벤토리에 있는지 먼저 확인
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemName == _item.itemName)
            {
                slots[i].SetSlotCount(_count);  // ✅ 개수만 증가
                slots[i].UpdateSlotUI();  // ✅ UI 업데이트
                return;
            }
        }

        // ✅ 같은 아이템이 없을 경우, 새로운 슬롯에 추가
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                slots[i].UpdateSlotUI();  // ✅ UI 업데이트
                return;
            }
        }
    }

    public bool HasItem(string itemName, int count)
    {
        // 인벤토리에 해당 아이템이 count 개수 이상 있는지 확인
        foreach (Slot slot in slots)
        {
            if (slot.item != null && slot.item.itemName == itemName)
            {
                if (slot.itemCount >= count)
                    return true;
            }
        }
        return false;
    }

    public bool ConsumeItem(string itemName, int count)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemName == itemName)
            {
                if (slots[i].itemCount >= count)
                {
                    slots[i].SetSlotCount(-count);  // ✅ 아이템 개수 감소
                    slots[i].UpdateSlotUI();  // ✅ UI 업데이트
                    return true;
                }
            }
        }
        return false;  // 아이템을 충분히 소모할 수 없으면 false 반환
    }





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    public static QuickSlotController instance; // ✅ 싱글턴

    [SerializeField]
    private Slot[] quickSlots;
    [SerializeField]
    private Transform tf_parent;

    private int selectedSlot;

    [SerializeField]
    private GameObject go_SelectedImage;
    [SerializeField]
    private WeaponManager1 theWeaponManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();

        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i].SetQuickSlotNumber(i); // ✅ 안전하게 넘겨줌
        }

        // ✅ 게임 시작 시 자동 장착
        if (quickSlots[selectedSlot].item != null && quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(
                quickSlots[selectedSlot].item.weaponType,
                quickSlots[selectedSlot].item.itemName));
        }
    }

    void Update()
    {
        TryInputNumber();
    }

    private void TryInputNumber()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeSlot(5);
    }

    public void IsActivatedQuickSlot(int _num)
    {
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }
        if (DragSlot.instance != null)
        {
            if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
            {
                Execute();
                return;
            }
        }
    }


    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Execute();
    }

    private void SelectedSlot(int _num)
    {
        selectedSlot = _num;
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    private void Execute()
    {
        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            else
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
        }
        else
        {
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
        }
    }

    //  현재 선택된 슬롯 번호 제공
    public int GetSelectedSlotNumber()
    {
        return selectedSlot;
    }

    //  퀵슬롯 배열 반환 함수 추가
    public Slot[] GetQuickSlots()
    {
        return quickSlots;
    }

    public void LoadQuickSlots(List<QuickSlotData> slotDataList)
    {
        for (int i = 0; i < quickSlots.Length && i < slotDataList.Count; i++)
        {
            var data = slotDataList[i];
            if (!string.IsNullOrEmpty(data.itemName))
            {
                Item item = ItemDatabase.instance.GetItemByName(data.itemName);
                if (item != null)
                {
                    quickSlots[i].AddItem(item, data.itemCount);
                }
            }
            else
            {
                quickSlots[i].ClearSlot();
            }
        }
    }


}

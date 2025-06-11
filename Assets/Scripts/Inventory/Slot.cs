using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// 보내주신 Slot.cs 코드 전체 (수정 없음)
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    //아이템
    public Item item;
    public int itemCount;
    public Image itemImage;
    public Text itemCountText;

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;


    private WeaponManager1 theWeaponManager;
    [SerializeField]
    private RectTransform baseRect;
    [SerializeField]
    private RectTransform quickSlotBaseRect;
    private InputNumber theInputNumber;
    private ItemEffectDataBase theItemEffectDatabase;


    [SerializeField]
    private bool isQuickSlot; // 퀵슬롯 여부
    [SerializeField]
    public int quickSlotNumber; // 퀵슬롯 넘버

    public bool canReceiveItem = true;

    public UnityEvent OnSlotChanged;

    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDataBase>();
        theWeaponManager = FindObjectOfType<WeaponManager1>();
        theInputNumber = FindObjectOfType<InputNumber>();

    }



    public void UpdateSlotUI()
    {
        if (item != null)
        {
            itemImage.sprite = item.itemImage;
            SetColor(1);

            if (item.itemType != Item.ItemType.Equipment)
            {
                go_CountImage.SetActive(true);
                text_Count.gameObject.SetActive(true);
                text_Count.text = itemCount.ToString();
            }
            else
            {
                go_CountImage.SetActive(false);
                text_Count.gameObject.SetActive(false);
                text_Count.text = "0";
            }
        }
        else
        {
            ClearSlot();
        }
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            theItemEffectDatabase.HideToolTip();
        }
    }


    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    public void AddItem(Item _item, int _count = 1)
    {
        if (!CanReceive(_item)) return; // 슬롯 필터에 걸리면 못 넣음

        ApplyItem(_item, _count);
    }

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }
    private IEnumerator DelayedEquip()
    {
        yield return new WaitForSeconds(0.1f); // 살짝만 지연

        if (QuickSlotController.instance != null &&
        quickSlotNumber == QuickSlotController.instance.GetSelectedSlotNumber())
        {
            if (item != null && item.itemType == Item.ItemType.Equipment)
            {
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
            }
        }

        // ✅ 선택된 퀵슬롯이면 자동 장착
        if (isQuickSlot && QuickSlotController.instance != null &&
            quickSlotNumber == QuickSlotController.instance.GetSelectedSlotNumber())
        {
            if (item.itemType == Item.ItemType.Equipment)
            {
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
            }
        }
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.gameObject.SetActive(true);
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
        OnSlotChanged?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            if (item.itemType == Item.ItemType.Equipment)
            {
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
            }
            else if (item.itemName == "BearMeat")
            {
                UseItem(); // UseItem 내부에서 상태 회복 + SetSlotCount 포함
            }
            else
            {
                SetSlotCount(-1); // 일반 소비 아이템
            }
        }
    }

    private void UseItem()
    {
        if (item.itemName == "BearMeat")
        {
            StatusController statusController = FindObjectOfType<StatusController>();
            if (statusController != null)
            {
                statusController.IncreaseHP(3);      //피 회복
                statusController.IncreaseHungry(10); //배고픔 회복
            }
            SetSlotCount(-1);
            UpdateSlotUI();
        }
    }



    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.originSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
            theItemEffectDatabase.HideToolTip();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        bool isInsideInventory = RectTransformUtility.RectangleContainsScreenPoint(baseRect, Input.mousePosition);
        bool isInsideQuickSlot = RectTransformUtility.RectangleContainsScreenPoint(quickSlotBaseRect, Input.mousePosition);

        if (!isInsideInventory && !isInsideQuickSlot)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                theInputNumber.OpenDropUI(DragSlot.instance.dragSlot);
            }
        }

        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;

        theItemEffectDatabase.HideToolTip();
        UpdateSlotUI();

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            if (isQuickSlot) //인벤토리 퀵슬롯
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            else //인벤토리 -> 인벤, 퀵슬롯 -> 인벤
                if (DragSlot.instance.dragSlot.isQuickSlot)
                theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
        }
    }

    private void ChangeSlot()
    {
        Item draggedItem = DragSlot.instance.dragSlot.item;
        int draggedCount = DragSlot.instance.dragSlot.itemCount;

        // 추가: 슬롯이 받을 수 있는지 확인
        if (!CanReceive(draggedItem))
        {
            Debug.LogWarning("이 슬롯은 해당 아이템을 받을 수 없습니다.");
            return;
        }

        // --- 디버깅을 위한 로그 추가 ---
        string originalItemName = (item != null) ? item.itemName : "비어있음";
        Debug.Log($"[슬롯 변경 시작] '{DragSlot.instance.dragSlot.name}'에서 '{name}'으로 '{draggedItem.itemName}' 옮기는 중. 대상 슬롯의 원래 아이템: '{originalItemName}'");


        // ✅ 이미 같은 아이템이 있다면 수량만 합치기
        if (item != null && item.itemName == draggedItem.itemName)
        {
            Debug.Log("같은 아이템 발견: 수량을 합칩니다.");
            itemCount += draggedCount;
            DragSlot.instance.dragSlot.ClearSlot(); // 원래 슬롯은 비워야 함
        }
        else
        {
            // 일반 교환
            Debug.Log("다른 아이템 발견: 아이템을 교환합니다.");
            Item _tempItem = item;
            int _tempItemCount = itemCount;

            ApplyItem(draggedItem, draggedCount);

            if (_tempItem != null)
            {
                Debug.Log($"원래 슬롯으로 '{_tempItem.itemName}' 아이템을 되돌려줍니다.");
                DragSlot.instance.dragSlot.ApplyItem(_tempItem, _tempItemCount);
            }
            else
            {
                Debug.Log("대상 슬롯이 비어있었으므로, 원래 슬롯을 비웁니다.");
                DragSlot.instance.dragSlot.ClearSlot();
            }
        }

        // 이 슬롯의 UI와 원래 슬롯의 UI를 모두 업데이트
        UpdateSlotUI();
        DragSlot.instance.dragSlot.UpdateSlotUI();
    }

    public void SetQuickSlotNumber(int _number)
    {
        quickSlotNumber = _number;
    }

    //마우스가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            theItemEffectDatabase.ShowToolTop(item, Input.mousePosition);
    }

    //슬롯에서 빠져나갈 때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }

    private void OnDisable()
    {
        if (theItemEffectDatabase != null)
            theItemEffectDatabase.HideToolTip(); // 슬롯 비활성화될 때도 툴팁 끄기
    }
    public virtual bool CanReceive(Item item)
    {
        return canReceiveItem;
    }
    public class OutputOnlySlot : Slot
    {
        public override bool CanReceive(Item item)
        {
            return false; // 어떤 아이템도 못 넣음
        }
    }
    public void ForceAddItem(Item _item, int _count = 1)
    {
        ApplyItem(_item, _count);
    }
    private void ApplyItem(Item _item, int _count)
    {
        if (_item.itemImage == null)
        {
            Debug.LogError($"[ApplyItem 오류] 슬롯 '{this.name}'에 아이템 '{_item.itemName}'을 적용하려 했으나, 이 아이템의 itemImage 필드가 비어있습니다(null)!");
        }
        else
        {
            Debug.Log($"[ApplyItem 정보] 슬롯 '{this.name}'에 아이템 '{_item.itemName}'의 이미지를 적용합니다. 이미지 이름: '{_item.itemImage.name}'");
        }
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;
        itemImage.gameObject.SetActive(true);

        if (item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.gameObject.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            go_CountImage.SetActive(false);
            text_Count.gameObject.SetActive(false);
            text_Count.text = "0";
        }
        
        SetColor(1);
        UpdateSlotUI();

        if (isQuickSlot)
            StartCoroutine(DelayedEquip());
        OnSlotChanged?.Invoke();
    }

    // ----- [ 조합 시스템 연동을 위해 추가된 함수들 ] -----

    public void RemoveItems(int amount)
    {
        itemCount -= amount;
        if (itemCount <= 0)
            ClearSlot();
        else
            UpdateSlotUI();
    }

    public void SetItem(Item newItem, int count)
    {
        AddItem(newItem, count);
    }

}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    //물통
    public bool isWaterFilled = false;
    public int waterAmount = 0;      // 현재 물의 양 (0 ~ maxWaterAmount)
    public int maxWaterAmount = 3;


    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDataBase>();
        theWeaponManager = FindObjectOfType<WeaponManager1>();
        theInputNumber = FindObjectOfType<InputNumber>();

    }



    public void UpdateSlotUI() // 이 메서드의 내부도 확인
    {
        if (item != null)
        {
            if (itemImage != null) // itemImage가 할당되어 있는지 확인
            {
                itemImage.sprite = item.itemImage;
                itemImage.gameObject.SetActive(true); // 이미지가 활성화되는지 확인
                SetColor(1);
            }

            if (item.itemType != Item.ItemType.Equipment)
            {
                if (go_CountImage != null) go_CountImage.SetActive(true);
                if (text_Count != null)
                {
                    text_Count.gameObject.SetActive(true);
                    text_Count.text = itemCount.ToString();
                }
            }
            else
            {
                if (go_CountImage != null) go_CountImage.SetActive(false);
                if (text_Count != null) text_Count.gameObject.SetActive(false);
            }
        }
        else // 아이템이 null이면 슬롯 초기화
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
        //if (!CanReceive(_item)) return; 이거 일단 임시로 함

        item = _item;
        itemCount = _count; // 초기 아이템 개수 설정
        isWaterFilled = false;
        UpdateSlotUI(); // <-- 이 부분이 UI 갱신을 담당합니다.
    }

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }
    private IEnumerator DelayedEquip()
    {
        yield return new WaitForSeconds(0.1f);

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
        if (itemCount <= 0)
            ClearSlot();
        else
            UpdateSlotUI();
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
                UseItem(); 
            }
            else
            {
                SetSlotCount(-1); 
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
                statusController.IncreaseHP(3);      
                statusController.IncreaseHungry(10); 
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
        if (!canReceiveItem)
        {
            return;
        }

        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            if (isQuickSlot)
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            else
                if (DragSlot.instance.dragSlot.isQuickSlot)
                theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
        }
    }

    private void ChangeSlot()
    {
        Item draggedItem = DragSlot.instance.dragSlot.item;
        int draggedCount = DragSlot.instance.dragSlot.itemCount;

        if (!CanReceive(draggedItem))
        {
            return;
        }

        string originalItemName = (item != null) ? item.itemName : "비어있음";


        if (item != null && item.itemName == draggedItem.itemName)
        {
            itemCount += draggedCount;
            DragSlot.instance.dragSlot.ClearSlot();
        }
        else
        {
            Item _tempItem = item;
            int _tempItemCount = itemCount;

            ApplyItem(draggedItem, draggedCount);

            if (_tempItem != null)
            {
                DragSlot.instance.dragSlot.ApplyItem(_tempItem, _tempItemCount);
            }
            else
            {
                DragSlot.instance.dragSlot.ClearSlot();
            }
        }
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
            theItemEffectDatabase.HideToolTip();
    }
    public virtual bool CanReceive(Item item)
    {
        return canReceiveItem;
    }
    public class OutputOnlySlot : Slot
    {
        public override bool CanReceive(Item item)
        {
            return false; // 어떤 아이템도 못 넣게함.
        }
    }
    public void ForceAddItem(Item _item, int _count = 1)
    {
        ApplyItem(_item, _count);
    }
    private void ApplyItem(Item _item, int _count)
    {
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
    public void UpdateBottleUI()
    {
        if (item != null && item.itemName == "IronBottle")
        {
            if (isWaterFilled)
            {
                itemImage.color = Color.cyan; 
            }
            else
            {
                itemImage.color = Color.white;
            }
        }
    }
}
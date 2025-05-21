using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        // ✅ 장착을 살짝 지연해서 시도 (Start 이후 실행되도록)
        if (isQuickSlot)
            StartCoroutine(DelayedEquip());

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



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
            theItemEffectDatabase.HideToolTip();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 localPos = DragSlot.instance.transform.position;

        Vector3[] inventoryCorners = new Vector3[4];
        baseRect.GetWorldCorners(inventoryCorners);
        bool isInsideInventory = (localPos.x > inventoryCorners[0].x && localPos.x < inventoryCorners[2].x
                                && localPos.y > inventoryCorners[0].y && localPos.y < inventoryCorners[2].y);

        Vector3[] quickSlotCorners = new Vector3[4];
        quickSlotBaseRect.GetWorldCorners(quickSlotCorners);
        bool isInsideQuickSlot = (localPos.x > quickSlotCorners[0].x && localPos.x < quickSlotCorners[2].x
                                && localPos.y > quickSlotCorners[0].y && localPos.y < quickSlotCorners[2].y);

        if (!(isInsideInventory || isInsideQuickSlot))
        {
            if (DragSlot.instance.dragSlot != null)
            {
                theInputNumber.Call(); // 버리기 UI
            }
        }
        else
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
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
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        UpdateSlotUI();

        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
            DragSlot.instance.dragSlot.UpdateSlotUI();
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
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

}

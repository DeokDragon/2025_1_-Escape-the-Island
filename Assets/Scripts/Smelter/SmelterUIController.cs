using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SmelterUIController : MonoBehaviour
{
    public Slot leftInputSlot;    // 왼쪽 제련 입력 슬롯
    public Slot rightOutputSlot;  // 오른쪽 제련 출력 슬롯 (아이템 넣기 막기)
    public TextMeshProUGUI statusText;    // 제련 상태 표시 텍스트

    private bool isSmelting = false;
    private bool isUIOpen = false;
    private float smeltingTime = 8f; // 제련 소요 시간 (초)

    [SerializeField] private GameObject smeltingUI;

    void Start()
    {
        rightOutputSlot.canReceiveItem = false;
    }

    public void OnStartSmeltingButton()
    {
        if (isSmelting) return; // 이미 제련 중이면 무시

        if (leftInputSlot.item == null)
        {
            statusText.text = "제련할 아이템을 넣어주세요.";
            return;
        }

        // 제련 가능한 아이템인지 검사
        if (CreateSmeltedItem(leftInputSlot.item) == null)
        {
            statusText.text = "제련할 수 없는 아이템입니다.";
            return;
        }

        if (rightOutputSlot.item != null)
        {
            Item predictedOutputItem = CreateSmeltedItem(leftInputSlot.item);
            if (predictedOutputItem != null && rightOutputSlot.item.itemName == predictedOutputItem.itemName)
            {
                // 스택이 가득 찼는지 확인 (Item 클래스에 maxStackCount 변수가 있다고 가정)
                if (rightOutputSlot.itemCount >= rightOutputSlot.item.maxStackCount)
                {
                    statusText.text = "출력 슬롯의 공간이 부족합니다.";
                    return;
                }
            }
            else // 기존 아이템과 새로 제련될 아이템이 다르다면 출력 슬롯 비워야 함
            {
                statusText.text = "출력 슬롯이 비어있지 않습니다.";
                return;
            }
        }

        SmelterCoroutineRunner.instance.StartCoroutine(SmeltingCoroutine());
    }

    IEnumerator SmeltingCoroutine()
    {
        isSmelting = true;

        Item inputItem = leftInputSlot.item;
        int initialInputCount = leftInputSlot.itemCount; // 초기에 슬롯에 있던 총 수량


        if (inputItem == null || initialInputCount <= 0)
        {
            statusText.text = "제련 실패: 입력 수량 오류";
            isSmelting = false;
            yield break;
        }

        Item outputItem = CreateSmeltedItem(inputItem);
        if (outputItem == null)
        {
            statusText.text = "제련할 수 없는 아이템입니다.";
            isSmelting = false;
            yield break;
        }

        int produced = 0;

        for (int i = 0; i < initialInputCount; i++) // 초기 입력 수량만큼 반복
        {
            if (leftInputSlot.item == null || leftInputSlot.itemCount <= 0 || leftInputSlot.item.itemName != inputItem.itemName)
            {
                statusText.text = $"제련 중단: 입력 아이템 부족.";
                isSmelting = false;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < smeltingTime) 
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / smeltingTime) * 100f;
                statusText.text = $"제련 중.. {i + 1} / {initialInputCount} ({percent:F0}%)";
                yield return null; 
            }
            // 1. 입력 슬롯에서 1개 감소
            leftInputSlot.SetSlotCount(-1);

            // 2. 결과 아이템을 출력 슬롯에 1개 추가
            Debug.Log($"[SmelterCoroutine] Right Slot Before Add/Set: Item='{(rightOutputSlot.item != null ? rightOutputSlot.item.itemName : "NULL")}', Count='{rightOutputSlot.itemCount}'"); //
            if (rightOutputSlot.item != null && rightOutputSlot.item.itemName == outputItem.itemName)
            {
                if (rightOutputSlot.itemCount < rightOutputSlot.item.maxStackCount)
                {
                    rightOutputSlot.SetSlotCount(1);
                }
            }
            else if (rightOutputSlot.item == null)
            {
                rightOutputSlot.AddItem(outputItem, 1);
            }
            produced++;
        }

        statusText.text = "제련 완료!";
        isSmelting = false;
    }

    private Item CreateSmeltedItem(Item inputItem)
    {
        Item smeltedItem = null;

        if (inputItem == null) return null;

        switch (inputItem.itemName) 
        {
            case "Iron Stone":
                smeltedItem = Resources.Load<Item>("Item/Iron"); //철
                break;
            case "AlloyOre":
                smeltedItem = Resources.Load<Item>("Item/Alloy_14"); //미스릴
                break;
            case "BearMeat":
                smeltedItem = Resources.Load<Item>("Item/GrilledBearMeat_21"); //곰고기
                break;
            case "WolfMeat":
                smeltedItem = Resources.Load<Item>("Item/GrilledWolfMeat_23"); //늑대고기
                break;
            default: // 위에 명시된 아이템 외에는 제련 불가
                return null; // 제련 불가능 아이템은 null 반환
        }

        return smeltedItem;
    }

    void Update()
    {
        if (rightOutputSlot.item != null && !isSmelting)
        {
            statusText.text = "제련 완료!";
        }
    }

    public void CloseSmeltingUI()
    {
        smeltingUI.SetActive(false);
        isUIOpen = false;

        GameManager.isSmeltingUIOpen = false;
        GameManager.UpdateCursorState();

        GameManager.canPlayerMove = true;
        GameManager.canPlayerRotate = true;
    }
}
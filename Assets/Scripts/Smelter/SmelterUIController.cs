using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SmelterUIController : MonoBehaviour
{
    public Slot leftInputSlot;    // 왼쪽 제련 입력 슬롯
    public Slot rightOutputSlot;  // 오른쪽 제련 출력 슬롯 (아이템 넣기 막기)
    public TextMeshProUGUI statusText;  // 제련 상태 표시 텍스트

    private bool isSmelting = false;
    private bool isUIOpen = false;
    private float smeltingTime = 8f; // 제련 소요 시간 (초)

    [SerializeField] private GameObject smeltingUI;

    void Start()
    {
        rightOutputSlot.canReceiveItem = false; // 출력 슬롯은 아이템 못 받게 설정
    }

    public void OnStartSmeltingButton()
    {
        Debug.Log("제련 버튼 눌림");
        if (isSmelting) return; // 이미 제련 중이면 무시

        if (leftInputSlot.item == null)
        {
            statusText.text = "제련할 아이템을 넣어주세요.";
            return;
        }

        if (rightOutputSlot.item != null)
        {
            statusText.text = "출력 슬롯이 비어있지 않습니다.";
            return;

        }
        SmelterCoroutineRunner.instance.StartCoroutine(SmeltingCoroutine());
    }

    IEnumerator SmeltingCoroutine()
    {
        isSmelting = true;

        Item inputItem = leftInputSlot.item;
        int inputCount = leftInputSlot.itemCount;

        if (inputItem == null || inputCount <= 0)
        {
            statusText.text = "제련 실패: 입력 수량 오류";
            isSmelting = false;
            yield break;
        }

        Item outputItem = CreateSmeltedItem(inputItem);
        leftInputSlot.ClearSlot(); // 슬롯 비움

        int produced = 0;

        for (int i = 0; i < inputCount; i++)
        {
            float elapsed = 0f;

            while (elapsed < smeltingTime)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / smeltingTime) * 100f;
                statusText.text = $"제련 중.. {i + 1} / {inputCount} ({percent:F0}%)";
                yield return null;
            }

            // 한 개씩 오른쪽 슬롯에 추가
            if (rightOutputSlot.item != null && rightOutputSlot.item.itemName == outputItem.itemName)
            {
                rightOutputSlot.SetSlotCount(1);
            }
            else if (rightOutputSlot.item == null)
            {
                rightOutputSlot.ForceAddItem(outputItem, 1);
            }

            produced++;
        }

        statusText.text = "제련 완료!";
        isSmelting = false;
    }

    private Item CreateSmeltedItem(Item inputItem)
    {
        Item smeltedItem = null;

        if (inputItem.itemName == "Iron Stone")
            smeltedItem = Resources.Load<Item>("Item/Iron");
        //else if (inputItem.itemName == "BearMeat")
        //    smeltedItem = Resources.Load<Item>("Item/Diamond");
        //else if (inputItem.itemName == "WolfMeat")
        //    smeltedItem = Resources.Load<Item>("Item/Diamond");

        if (smeltedItem == null)
        {
            Debug.LogWarning("[Smelter] 제련 아이템 불러오기 실패");
            smeltedItem = ScriptableObject.CreateInstance<Item>();
            smeltedItem.itemName = inputItem.itemName + " Ingot";
        }
        else
        {
            Debug.Log("[Smelter] 아이템 불러오기 성공: " + smeltedItem.itemName);

            Debug.Log("불러온 아이템 스프라이트: " + smeltedItem.itemImage);
        }

        return smeltedItem;
    }

    void Update()
    {
        // 오른쪽 슬롯에 아이템 있으면 상태 항상 "제련 완료!" 표시
        if (rightOutputSlot.item != null && !isSmelting)
        {
            statusText.text = "제련 완료!";
        }
    }
    public void CloseSmeltingUI()
    {
        smeltingUI.SetActive(false);
        isUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.canPlayerMove = true;
        GameManager.canPlayerRotate = true;
    }

}
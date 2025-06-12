using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]

public class ToolUpgradeRecipe
{
    public string inputItemName;
    public string resultItemName;
    public Dictionary<string, int> requiredMaterials;

    public ToolUpgradeRecipe(string inputName, string resultName, Dictionary<string, int> materials)
    {
        inputItemName = inputName.Trim();
        resultItemName = resultName.Trim();
        requiredMaterials = materials;

        Debug.Log("[레시피 등록됨] input: " + inputItemName + ", result: " + resultItemName);
    }

    // 2. override 키워드를 public으로 변경
    public bool CheckRecipe(Slot toolSlot, Slot[] materialSlots)
    {
        if (toolSlot.item == null) return false;

        if (!string.Equals(toolSlot.item.itemName.Trim(), inputItemName.Trim(), System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"[레시피 불일치] 슬롯 아이템: {toolSlot.item.itemName} / 레시피 기대값: {inputItemName}");
            return false;
        }
        return true;
    }

    // 3. override 키워드를 public으로 변경
    public void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot)
    {
        // 1. 조합 조건 확인
        if (!CheckRecipe(toolSlot, materialSlots))
        {
            Debug.LogWarning("[Craft 실패] 조건 불충족");
            return;
        }

        // 2. 재료 소모
        foreach (var req in requiredMaterials)
        {
            int need = req.Value;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    int remove = Mathf.Min(slot.itemCount, need);

                    slot.SetSlotCount(-remove);

                    need -= remove;
                    if (need <= 0)
                        break;
                }
            }
        }

        // 3. 결과 아이템 생성
        // ItemDatabase를 싱글톤으로 만들었거나 Resources.Load 등을 통해 아이템을 가져옵니다.
        Item resultItem = Resources.Load<Item>("Item/" + resultItemName);
        if (resultItem == null)
        {
            Debug.LogError($"[Craft 실패] 결과 아이템 로드 실패: {resultItemName}");
            return;
        }

        // 4. 결과 슬롯에 아이템 설정 (UI 업데이트 포함)
        resultSlot.SetItem(resultItem, 1);

        // 5. 기존 도구 슬롯 비우기 (UI 업데이트 포함)
        toolSlot.ClearSlot();

        Debug.Log($"[강화 완료] {inputItemName} → {resultItemName}");
    }

    // 4. override 키워드를 public으로 변경
    public Item GetPreviewResult()
    {
        return Resources.Load<Item>("Item/" + resultItemName);
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// BaseRecipe를 상속받는 업그레이드 전용 레시피 클래스
public class AnvilUpgradeRecipe : BaseRecipe
{
    [Header("업그레이드할 도구")]
    public Item toolToUpgrade;

    [Header("업그레이드 결과물")]
    public Item resultItem;

    [Header("필요한 재료 목록")]
    public MaterialRequirement[] requiredMaterials;


    // [조합 조건 확인]
    public override bool CheckRecipe(Slot toolSlot, Slot[] materialSlots)
    {
        // 1. 도구 슬롯 확인
        if (toolSlot == null || toolSlot.item != toolToUpgrade)
        {
            return false;
        }

        // 2. 재료 확인
        foreach (var req in requiredMaterials)
        {
            // 'quantity'를 'itemCount'로 수정
            int currentAmount = materialSlots
                                .Where(slot => slot != null && slot.item == req.item)
                                .Sum(slot => slot.itemCount);

            if (currentAmount < req.quantity)
            {
                return false;
            }
        }

        return true;
    }

    // [조합 실행]
    public override void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot)
    {
        if (!CheckRecipe(toolSlot, materialSlots)) return;

        // 1. 기존 도구 소모 (ClearSlot 함수는 이미 존재)
        toolSlot.ClearSlot();

        // 2. 재료 소모
        foreach (var req in requiredMaterials)
        {
            int amountToConsume = req.quantity;
            foreach (var matSlot in materialSlots)
            {
                if (matSlot != null && matSlot.item == req.item)
                {
                    // 'quantity'를 'itemCount'로 수정
                    if (matSlot.itemCount >= amountToConsume)
                    {
                        // 'RemoveItems' 함수를 호출하도록 수정 (내부에서는 SetSlotCount가 동작)
                        matSlot.RemoveItems(amountToConsume);
                        amountToConsume = 0;
                    }
                    else
                    {
                        amountToConsume -= matSlot.itemCount;
                        matSlot.ClearSlot();
                    }
                }
                if (amountToConsume == 0) break;
            }
        }

        // 3. 결과물 슬롯에 새로운 아이템 생성
        // 'SetItem' 함수를 호출하도록 수정 (내부에서는 ApplyItem이 동작)
        resultSlot.SetItem(resultItem, 1);
    }

    // [미리보기 제공]
    public override Item GetPreviewResult()
    {
        return resultItem;
    }
}
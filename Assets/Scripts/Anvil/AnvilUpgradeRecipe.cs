using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnvilUpgradeRecipe : BaseRecipe
{
    [Header("업그레이드할 도구")]
    public Item toolToUpgrade;

    [Header("업그레이드 결과물")]
    public Item resultItem;

    [Header("필요한 재료 목록")]
    public MaterialRequirement[] requiredMaterials;

    public override bool CheckRecipe(Slot toolSlot, Slot[] materialSlots)
    {
        if (toolSlot == null || toolSlot.item != toolToUpgrade)
        {
            return false;
        }

        foreach (var req in requiredMaterials)
        {
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

    public override void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot)
    {
        if (!CheckRecipe(toolSlot, materialSlots)) return;

        toolSlot.ClearSlot();

        foreach (var req in requiredMaterials)
        {
            int amountToConsume = req.quantity;
            foreach (var matSlot in materialSlots)
            {
                if (matSlot != null && matSlot.item == req.item)
                {
                    if (matSlot.itemCount >= amountToConsume)
                    {
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

        resultSlot.SetItem(resultItem, 1);
    }

    public override Item GetPreviewResult()
    {
        return resultItem;
    }
}
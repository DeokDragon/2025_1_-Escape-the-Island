using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolOnlySlot : Slot
{
    public override bool CanReceive(Item item)
    {
        return item.itemType == Item.ItemType.Equipment && IsValidUpgradeableTool(item.itemName);
    }

    private bool IsValidUpgradeableTool(string itemName)
    {
        foreach (var recipe in ToolUpgradeDatabase.AllRecipes)
        {
            if (recipe.inputItemName == itemName)
                return true;
        }
        return false;
    }
}

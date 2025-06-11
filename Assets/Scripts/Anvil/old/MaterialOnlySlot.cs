using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOnlySlot : Slot
{
    private static readonly HashSet<string> allowedMaterials = new HashSet<string>
    {
        "철", "다이아몬드", "나무", "밧줄"
    };

    public override bool CanReceive(Item item)
    {
        return item != null && allowedMaterials.Contains(item.itemName);
    }
}
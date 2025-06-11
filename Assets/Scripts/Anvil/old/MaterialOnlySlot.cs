using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOnlySlot : Slot
{
    private static readonly HashSet<string> allowedMaterials = new HashSet<string>
    {
        "ö", "���̾Ƹ��", "����", "����"
    };

    public override bool CanReceive(Item item)
    {
        return item != null && allowedMaterials.Contains(item.itemName);
    }
}
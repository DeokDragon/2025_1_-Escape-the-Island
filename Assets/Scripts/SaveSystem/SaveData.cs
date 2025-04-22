using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Vector3 playerPosition;

    public float hp;
    public float stamina;
    public float hunger;
    public float thirst;

    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();

    public string equippedWeaponName;

    public float currentTime;

    public int currentWoodCount;
}

[Serializable]
public class InventorySlotData
{
    public string itemName;
    public int itemCount;
}

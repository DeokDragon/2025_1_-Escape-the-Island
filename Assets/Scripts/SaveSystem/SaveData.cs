using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int caveIndex;
    public Vector3 playerPosition;
    public float hp;
    public float stamina;
    public float hunger;
    public float thirst;

    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();
    public string equippedWeaponName;

    public float currentTime;
    public int currentWoodCount;
    public int currentShipRepairStage;

    public List<QuickSlotData> quickSlots = new List<QuickSlotData>();
    public List<QuickSlotData> quickSlotDataList = new List<QuickSlotData>();
    public List<SpawnedObjectData> spawnedObjects = new List<SpawnedObjectData>();
}

[System.Serializable]
public class InventorySlotData
{
    public string itemName;
    public int itemCount;
}

[System.Serializable]
public class QuickSlotData
{
    public string itemName;
    public int itemCount;
}

[System.Serializable]
public class SpawnedObjectData
{
    public string prefabName;
    public Vector3 position;
    public Vector3 rotation;
}

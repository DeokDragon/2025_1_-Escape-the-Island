using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int caveIndex; // 활성화된 동굴의 인덱스 저장

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

    public List<QuickSlotData> quickSlots = new List<QuickSlotData>(); // ✅ 퀵슬롯 정보 추가
    public List<QuickSlotData> quickSlotDataList = new List<QuickSlotData>(); 
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




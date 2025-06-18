using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string itemName;
    public string itemDesc;
    public string itemType;
    public string itemImagePath;
    public string itemPrefabPath;
    public string weaponType;
    public float attackPower;
    public string materialType;
    public int maxStackCount;
}

[Serializable]
public class ItemDataListWrapper
{
    public List<ItemData> items;
}

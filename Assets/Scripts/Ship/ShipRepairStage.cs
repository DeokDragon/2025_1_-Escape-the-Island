using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipRepairStage
{
    public int stageLevel;
    public string stageName;
    public Dictionary<string, int> requiredMaterials;
    public GameObject shipAppearance; // 해당 단계에서 보여줄 배의 외형 GameObject

    public ShipRepairStage(int level, string name, Dictionary<string, int> materials, GameObject appearance = null)
    {
        stageLevel = level;
        stageName = name;
        requiredMaterials = materials;
        shipAppearance = appearance;
    }
}

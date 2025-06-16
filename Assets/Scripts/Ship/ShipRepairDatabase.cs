using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShipRepairDatabase
{
    public static Dictionary<int, ShipRepairStage> AllStages = new Dictionary<int, ShipRepairStage>
    {
        { 2, new ShipRepairStage(2, "���� 1�ܰ�", new Dictionary<string, int>
            {
                { "Log", 40 },
                { "Rock", 30 }
            })
        },
        { 3, new ShipRepairStage(3, "���� 2�ܰ�", new Dictionary<string, int>
            {
                { "Board", 40 },      
                { "Log", 20 },
                { "Mot", 20 }         
            })
        },
        { 4, new ShipRepairStage(4, "���� 3�ܰ�", new Dictionary<string, int>
            {
                { "Mot", 25 },        
                { "Board", 40 },      
                { "Iron", 40 },
                { "Alloy", 20 },
                { "Rope", 30 }
            })
        },
        { 5, new ShipRepairStage(5, "���� 4�ܰ�", new Dictionary<string, int>
            {
                { "Mot", 40 },        
                { "Board", 50 },      
                { "Alloy", 35 },
                { "Diamond", 40 }
            })
        }
    };

    public static ShipRepairStage GetStageInfo(int stageLevel)
    {
        if (AllStages.ContainsKey(stageLevel))
        {
            return AllStages[stageLevel];
        }
        return null;
    }
}
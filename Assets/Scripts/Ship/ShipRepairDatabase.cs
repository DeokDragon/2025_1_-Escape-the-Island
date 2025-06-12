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
                { "Board", 40 },      // Wood Plank -> Board
                { "Log", 20 },
                { "Mot", 20 }         // Mot���� ����
            })
        },
        { 4, new ShipRepairStage(4, "���� 3�ܰ�", new Dictionary<string, int>
            {
                { "Mot", 25 },        // Nail -> Mot
                { "Board", 40 },      // Wood Plank -> Board
                { "Iron", 40 },
                { "Diamond", 10 },
                { "Rope", 30 }
            })
        },
        { 5, new ShipRepairStage(5, "���� 4�ܰ�", new Dictionary<string, int>
            {
                { "Mot", 40 },        // Nail -> Mot
                { "Board", 50 },      // Wood Plank -> Board
                { "Iron", 50 },
                { "Diamond", 30 }
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
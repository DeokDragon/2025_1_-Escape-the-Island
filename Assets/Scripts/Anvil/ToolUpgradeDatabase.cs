using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public static class ToolUpgradeDatabase
{
    public static List<ToolUpgradeRecipe> AllRecipes = new List<ToolUpgradeRecipe>
    {
        // 돌곡괭이 → 철곡괭이
        new ToolUpgradeRecipe("RockPickaxe", "IronPickaxe", new Dictionary<string, int>
        {
            { "Iron", 20 },
            { "Log", 20 },
            { "Rope", 10 }
        }),

        // 돌도끼 → 철도끼
        new ToolUpgradeRecipe("RockAxe", "IronAxe", new Dictionary<string, int>
        {
            { "Iron", 20 },
            { "Log", 20 },
            { "Rope", 10 }
        }),

        // 철곡괭이 → 다이아곡괭이
        new ToolUpgradeRecipe("IronPickaxe", "DiamondPickaxe", new Dictionary<string, int>
        {
            { "Diamond", 35 },
            { "Log", 30 },
            { "Rope", 30 }
        }),

        // 철도끼 → 다이아도끼
        new ToolUpgradeRecipe("IronAxe", "DiamondAxe", new Dictionary<string, int>
        {
            { "Diamond", 35 },
            { "Log", 30 },
            { "Rope", 30 }
        })
    };

    // 🔍 itemName으로 레시피 찾는 함수
    public static ToolUpgradeRecipe GetRecipe(string inputItemName)
    {
        foreach (var recipe in AllRecipes)
        {
            if (recipe.inputItemName.Trim().ToLower() == inputItemName.Trim().ToLower())
            {
                Debug.Log("[GetRecipe 매칭됨] " + recipe.inputItemName + " == " + inputItemName);
                return recipe;
            }
            else
            {
                Debug.Log("[GetRecipe 비교 실패] " + recipe.inputItemName + " != " + inputItemName);
            }
        }

        Debug.Log("[GetRecipe 실패] 찾는 도구 이름: " + inputItemName);
        return null;
    }
}
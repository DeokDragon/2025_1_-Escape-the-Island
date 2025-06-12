using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AnvilUIController : MonoBehaviour
{
    [SerializeField] private Slot[] materialSlots;
    [SerializeField] private Slot resultSlot;
    [SerializeField] private Slot toolSlot;
    [SerializeField] private Slot previewSlot;

    [SerializeField] private Slot[] recipeDisplaySlots;

    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private Button[] filterButtons;

    private bool isUpgrading = false;

    [SerializeField] private GameObject previewPanel;

    [SerializeField] private GameObject go_AnvilUIBase;

    void Start()
    {
        var dummy = ToolUpgradeDatabase.AllRecipes.Count; // 강제 참조
    }
    IEnumerator UpgradeCoroutine(ToolUpgradeRecipe recipe)
    {
        isUpgrading = true;
        statusText.text = "업그레이드 중...";

        yield return new WaitForSeconds(2f);

        ConsumeMaterials(recipe);

        Item upgradedItem = Instantiate(Resources.Load<Item>("Item/" + recipe.resultItemName));

        upgradedItem.attackPower += 10;
        upgradedItem.materialType = Item.MaterialType.Diamond;

        resultSlot.ForceAddItem(upgradedItem, 1);

        statusText.text = "업그레이드 완료!";
        isUpgrading = false;
    }

    bool HasRequiredMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var requirement in recipe.requiredMaterials)
        {
            bool found = false;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null &&
                    slot.item.itemName == requirement.Key &&
                    slot.itemCount >= requirement.Value)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    void ConsumeMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var key in recipe.requiredMaterials.Keys.ToList())
        {
            int remaining = recipe.requiredMaterials[key];

            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == key)
                {
                    int consume = Mathf.Min(slot.itemCount, remaining);
                    slot.SetSlotCount(-consume);
                    remaining -= consume;

                    if (recipe.requiredMaterials.ContainsKey(key))
                    {
                        recipe.requiredMaterials[key] = remaining;
                    }

                    if (remaining <= 0)
                        break;
                }
            }
        }
    }
    public void FilterList(string type)
    {
    }
    public void OnUpgradeButton()
    {
        Item baseTool = toolSlot.item;
        if (baseTool == null)
        {
            statusText.text = "도구를 넣어주세요.";
            return;
        }

        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == baseTool.itemName);
        if (recipe == null)
        {
            statusText.text = "이 도구는 강화할 수 없습니다.";
            return;
        }

        if (!HasEnoughMaterials(recipe))
        {
            statusText.text = "재료가 부족합니다.";
            return;
        }

        recipe.Craft(toolSlot, materialSlots, resultSlot);
        statusText.text = "강화 완료!";
    }
    bool HasEnoughMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var req in recipe.requiredMaterials)
        {
            int totalInSlots = 0;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    totalInSlots += slot.itemCount;
                }
            }

            if (totalInSlots < req.Value)
                return false;
        }
        return true;
    }
    private ToolUpgradeRecipe FindMatchingRecipe(string baseItemName)
    {
        foreach (var recipe in ToolUpgradeDatabase.AllRecipes)
        {
            if (recipe.inputItemName == baseItemName)
                return recipe;
        }
        return null;
    }

    private void UpdatePreview()
    {
        previewPanel.SetActive(false);
        previewSlot.ClearSlot();

        if (toolSlot.item == null) return;

        var recipe = FindMatchingRecipe(toolSlot.item.itemName);
        if (recipe != null && recipe.CheckRecipe(toolSlot, materialSlots))
        {
            Item previewItem = recipe.GetPreviewResult();
            if (previewItem != null)
            {
                previewSlot.ForceAddItem(previewItem, 1);
                previewPanel.SetActive(true);
            }
        }
    }
    public void RefreshPreview()
    {
        UpdatePreview();
    }
    public void UpdateAnvilPreview()
    {
        if (toolSlot == null) { return; }
        if (previewSlot == null) { return; }
        if (recipeDisplaySlots == null) { return; }

        previewSlot.ClearSlot();
        foreach (var slot in recipeDisplaySlots)
        {
            if (slot != null) slot.ClearSlot();
            else { return; }
        }

        if (toolSlot.item == null)
        {
            return;
        }

        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == toolSlot.item.itemName);
        if (recipe == null)
        {
            return;
        }

        Item resultItem = Resources.Load<Item>("Item/" + recipe.resultItemName);
        if (resultItem != null)
        {
            previewSlot.SetItem(resultItem, 1);
        }

        int i = 0;
        foreach (var requiredMaterial in recipe.requiredMaterials)
        {
            if (i >= recipeDisplaySlots.Length)
            {
                break;
            }
            string materialName = requiredMaterial.Key;
            int materialAmount = requiredMaterial.Value;
            Item materialItem = Resources.Load<Item>("Item/" + materialName);

            if (materialItem != null)
            {
                recipeDisplaySlots[i].ForceAddItem(materialItem, materialAmount);
            }
            i++;
        }
    }
}
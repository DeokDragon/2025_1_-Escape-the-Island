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
        Debug.Log("Anvil UI ����");
        var dummy = ToolUpgradeDatabase.AllRecipes.Count; // ���� ����
    }
    IEnumerator UpgradeCoroutine(ToolUpgradeRecipe recipe)
    {
        isUpgrading = true;
        statusText.text = "���׷��̵� ��...";

        yield return new WaitForSeconds(2f); // ���׷��̵� ��� �ð�

        // ��� �Һ�
        ConsumeMaterials(recipe);

        // ��� ������ �ε�
        Item upgradedItem = Instantiate(Resources.Load<Item>("Item/" + recipe.resultItemName));

        // ��ȭ�� ������ �Ӽ� ����
        upgradedItem.attackPower += 10;  // ���ݷ� ���� (����)
        upgradedItem.materialType = Item.MaterialType.Diamond;  // ���͸��� ���� (enum ����)

        // ��� ���Կ� �߰�
        resultSlot.ForceAddItem(upgradedItem, 1);

        statusText.text = "���׷��̵� �Ϸ�!";
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
        foreach (var key in recipe.requiredMaterials.Keys.ToList()) // ToList()�� ���� ����
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
        // ��: type == "Axe" �Ǵ� "Pickaxe"
        // �� �Լ��� 5�� ��ư���� onClick���� �����
        // ��� ǥ�ø� �����ϴ� �뵵
    }
    public void OnUpgradeButton()
    {
        Debug.Log("[�˻��] toolSlot.item.itemName = " + toolSlot.item.itemName);
        Item baseTool = toolSlot.item;
        if (baseTool == null)
        {
            Debug.Log("[��ȭ ����] ���� ����");
            statusText.text = "������ �־��ּ���.";
            return;
        }

        Debug.Log("[��ȭ �õ�] ���� �̸�: " + baseTool.itemName);

        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == baseTool.itemName);
        if (recipe == null)
        {
            Debug.Log("[��ȭ ����] ������ ����: " + baseTool.itemName);
            statusText.text = "�� ������ ��ȭ�� �� �����ϴ�.";
            return;
        }

        if (!HasEnoughMaterials(recipe))
        {
            Debug.Log("[��ȭ ����] ��� ����");
            statusText.text = "��ᰡ �����մϴ�.";
            return;
        }

        Debug.Log("[��ȭ ���� ���� ����]");

        // ��ȭ ����
        recipe.Craft(toolSlot, materialSlots, resultSlot);
        statusText.text = "��ȭ �Ϸ�!";
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

        // ������ ã��
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
        Debug.Log("--- [1] UpdateAnvilPreview �Լ� ���� ---");

        // --- ���� ���� Ȯ�� ---
        if (toolSlot == null) { Debug.LogError("����: Tool Slot ��ü�� �ν����Ϳ� ������� �ʾҽ��ϴ�!"); return; }
        if (previewSlot == null) { Debug.LogError("����: Preview Slot (����ĭ)�� ������� �ʾҽ��ϴ�!"); return; }
        if (recipeDisplaySlots == null) { Debug.LogError("����: Recipe Display Slots (��ũĭ) �迭�� ������� �ʾҽ��ϴ�!"); return; }

        // --- ���� ���� �ʱ�ȭ ---
        previewSlot.ClearSlot();
        foreach (var slot in recipeDisplaySlots)
        {
            if (slot != null) slot.ClearSlot();
            else { Debug.LogError("����: Recipe Display Slots �迭�� ����ִ�(None) ĭ�� �ֽ��ϴ�!"); return; }
        }
        Debug.Log("[2] ��� �̸�����/��� ǥ�� ������ �ʱ�ȭ�߽��ϴ�.");

        // --- ���� Ȯ�� ---
        if (toolSlot.item == null)
        {
            Debug.LogWarning("[3] Tool Slot�� �������� �����ϴ�. ���⼭ �Լ��� �����մϴ�.");
            return;
        }
        Debug.Log($"[3] Tool Slot ������ Ȯ��: '{toolSlot.item.itemName}'");

        // --- ������ �˻� ---
        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == toolSlot.item.itemName);
        if (recipe == null)
        {
            Debug.LogError($"[4] ����: '{toolSlot.item.itemName}'�� ���� �����Ǹ� �����ͺ��̽����� ã�� �� �����ϴ�!");
            return;
        }
        Debug.Log($"[4] ������ �߰�! �����: '{recipe.resultItemName}'");

        // --- �̸�����(����ĭ) ������Ʈ ---
        Debug.Log("[5] �̸�����(����ĭ) ������Ʈ�� �õ��մϴ�...");
        Item resultItem = Resources.Load<Item>("Item/" + recipe.resultItemName);
        if (resultItem == null)
        {
            Debug.LogError($"[5] ����: Resources �������� ����� '{recipe.resultItemName}'�� �ε��� �� �����ϴ�! ��ο� ���ϸ��� Ȯ���ϼ���.");
        }
        else
        {
            previewSlot.SetItem(resultItem, 1);
            Debug.Log($"[5] ����: �̸����⿡ '{resultItem.itemName}'�� ǥ���߽��ϴ�.");
        }

        // --- �ʿ� ���(��ũĭ) ������Ʈ ---
        Debug.Log($"[6] �ʿ� ���(��ũĭ) ������Ʈ�� �õ��մϴ�... (�� {recipe.requiredMaterials.Count} ����)");
        int i = 0;
        foreach (var requiredMaterial in recipe.requiredMaterials)
        {
            if (i >= recipeDisplaySlots.Length)
            {
                Debug.LogWarning($"[6] ���: ��� ǥ�� ����(��ũĭ)�� �����Ͽ� ��� ��Ḧ ǥ���� �� �����ϴ�.");
                break;
            }
            string materialName = requiredMaterial.Key;
            int materialAmount = requiredMaterial.Value;
            Item materialItem = Resources.Load<Item>("Item/" + materialName);

            if (materialItem == null)
            {
                // ������ �ε忡 �����ϸ� ���� �α׸� �����, ���Կ� �ƹ� �۾��� ���� �ʽ��ϴ�.
                Debug.LogError($"[6] ����: Resources �������� ��� '{materialName}'�� �ε��� �� �����ϴ�!");
            }
            else
            {
                // --- �ٷ� �� �κ��Դϴ�! ---
                // SetItem�� ForceAddItem���� �����Ͽ� 'CanReceive' �˻縦 �����ϰ� ������ �������� ǥ���մϴ�.
                recipeDisplaySlots[i].ForceAddItem(materialItem, materialAmount);
                Debug.Log($"  - {i}�� ��ũ ���Կ� '{materialName}' {materialAmount}�� ǥ�� �Ϸ�.");
            }
            i++;
        }

        Debug.Log("--- [7] UpdateAnvilPreview �Լ� ���� ���� ---");
    }
}


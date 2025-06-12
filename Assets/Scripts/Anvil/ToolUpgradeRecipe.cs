using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]

public class ToolUpgradeRecipe
{
    public string inputItemName;
    public string resultItemName;
    public Dictionary<string, int> requiredMaterials;

    public ToolUpgradeRecipe(string inputName, string resultName, Dictionary<string, int> materials)
    {
        inputItemName = inputName.Trim();
        resultItemName = resultName.Trim();
        requiredMaterials = materials;

        Debug.Log("[������ ��ϵ�] input: " + inputItemName + ", result: " + resultItemName);
    }

    // 2. override Ű���带 public���� ����
    public bool CheckRecipe(Slot toolSlot, Slot[] materialSlots)
    {
        if (toolSlot.item == null) return false;

        if (!string.Equals(toolSlot.item.itemName.Trim(), inputItemName.Trim(), System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"[������ ����ġ] ���� ������: {toolSlot.item.itemName} / ������ ��밪: {inputItemName}");
            return false;
        }
        return true;
    }

    // 3. override Ű���带 public���� ����
    public void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot)
    {
        // 1. ���� ���� Ȯ��
        if (!CheckRecipe(toolSlot, materialSlots))
        {
            Debug.LogWarning("[Craft ����] ���� ������");
            return;
        }

        // 2. ��� �Ҹ�
        foreach (var req in requiredMaterials)
        {
            int need = req.Value;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    int remove = Mathf.Min(slot.itemCount, need);

                    slot.SetSlotCount(-remove);

                    need -= remove;
                    if (need <= 0)
                        break;
                }
            }
        }

        // 3. ��� ������ ����
        // ItemDatabase�� �̱������� ������ų� Resources.Load ���� ���� �������� �����ɴϴ�.
        Item resultItem = Resources.Load<Item>("Item/" + resultItemName);
        if (resultItem == null)
        {
            Debug.LogError($"[Craft ����] ��� ������ �ε� ����: {resultItemName}");
            return;
        }

        // 4. ��� ���Կ� ������ ���� (UI ������Ʈ ����)
        resultSlot.SetItem(resultItem, 1);

        // 5. ���� ���� ���� ���� (UI ������Ʈ ����)
        toolSlot.ClearSlot();

        Debug.Log($"[��ȭ �Ϸ�] {inputItemName} �� {resultItemName}");
    }

    // 4. override Ű���带 public���� ����
    public Item GetPreviewResult()
    {
        return Resources.Load<Item>("Item/" + resultItemName);
    }
}
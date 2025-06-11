using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// BaseRecipe�� ��ӹ޴� ���׷��̵� ���� ������ Ŭ����
public class AnvilUpgradeRecipe : BaseRecipe
{
    [Header("���׷��̵��� ����")]
    public Item toolToUpgrade;

    [Header("���׷��̵� �����")]
    public Item resultItem;

    [Header("�ʿ��� ��� ���")]
    public MaterialRequirement[] requiredMaterials;


    // [���� ���� Ȯ��]
    public override bool CheckRecipe(Slot toolSlot, Slot[] materialSlots)
    {
        // 1. ���� ���� Ȯ��
        if (toolSlot == null || toolSlot.item != toolToUpgrade)
        {
            return false;
        }

        // 2. ��� Ȯ��
        foreach (var req in requiredMaterials)
        {
            // 'quantity'�� 'itemCount'�� ����
            int currentAmount = materialSlots
                                .Where(slot => slot != null && slot.item == req.item)
                                .Sum(slot => slot.itemCount);

            if (currentAmount < req.quantity)
            {
                return false;
            }
        }

        return true;
    }

    // [���� ����]
    public override void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot)
    {
        if (!CheckRecipe(toolSlot, materialSlots)) return;

        // 1. ���� ���� �Ҹ� (ClearSlot �Լ��� �̹� ����)
        toolSlot.ClearSlot();

        // 2. ��� �Ҹ�
        foreach (var req in requiredMaterials)
        {
            int amountToConsume = req.quantity;
            foreach (var matSlot in materialSlots)
            {
                if (matSlot != null && matSlot.item == req.item)
                {
                    // 'quantity'�� 'itemCount'�� ����
                    if (matSlot.itemCount >= amountToConsume)
                    {
                        // 'RemoveItems' �Լ��� ȣ���ϵ��� ���� (���ο����� SetSlotCount�� ����)
                        matSlot.RemoveItems(amountToConsume);
                        amountToConsume = 0;
                    }
                    else
                    {
                        amountToConsume -= matSlot.itemCount;
                        matSlot.ClearSlot();
                    }
                }
                if (amountToConsume == 0) break;
            }
        }

        // 3. ����� ���Կ� ���ο� ������ ����
        // 'SetItem' �Լ��� ȣ���ϵ��� ���� (���ο����� ApplyItem�� ����)
        resultSlot.SetItem(resultItem, 1);
    }

    // [�̸����� ����]
    public override Item GetPreviewResult()
    {
        return resultItem;
    }
}
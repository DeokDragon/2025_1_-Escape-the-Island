using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ���չ� ��ũ��Ʈ�� ��ӹ޾ƾ� �� �߻� Ŭ����
// MonoBehaviour�� ��ӹ޾� ���� ������Ʈ�� ���� �� �ֵ��� ��
public abstract class BaseRecipe : MonoBehaviour
{
    public abstract bool CheckRecipe(Slot toolSlot, Slot[] materialSlots);

    public abstract void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot);

    public abstract Item GetPreviewResult();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ���չ� ��ũ��Ʈ�� ��ӹ޾ƾ� �� �߻� Ŭ����
// MonoBehaviour�� ��ӹ޾� ���� ������Ʈ�� ���� �� �ֵ��� ��
public abstract class BaseRecipe : MonoBehaviour
{
    // [���� ���� Ȯ��]
    // �� �����ǰ� ���� ���� ���¿��� ����� �� �ִ��� Ȯ���մϴ�.
    // �� ���չ� ��ũ��Ʈ���� �� �κ��� �ڽſ� �°� �����ؾ� �մϴ�.
    public abstract bool CheckRecipe(Slot toolSlot, Slot[] materialSlots);

    // [���� ����]
    // ���� ������ �����ϰ�, ��Ḧ �Ҹ��ϸ�, ������� �����մϴ�.
    public abstract void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot);

    // [�̸����� ����]
    // ���� ������� �������� �̸���������� ��ȯ�մϴ�.
    public abstract Item GetPreviewResult();
}
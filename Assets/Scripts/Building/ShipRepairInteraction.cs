using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairInteraction : MonoBehaviour
{
    public GameObject ship; // ���ļ� ������Ʈ
    private bool isNearShip = false; // �÷��̾ ���ļ� ��ó�� �ִ��� ����

    void Update()
    {
        if (isNearShip && Input.GetKeyDown(KeyCode.E))
        {
            // E Ű�� ������ ���� ����
            RepairShip();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ ��ó�� ������ ��ȣ�ۿ� ����
            isNearShip = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ ������ ��ȣ�ۿ� �Ұ���
            isNearShip = false;
        }
    }

    void RepairShip()
    {
        // �κ��丮���� ������ �ִ��� Ȯ���ϰ�, ������ ������ ���� ����
        if (Inventory.instance.HasItem("Log", 1))
        {
            // ���� 1�� �Ҹ� �� ���� ����
            Inventory.instance.ConsumeItem("Log", 1);
        }
    }
}
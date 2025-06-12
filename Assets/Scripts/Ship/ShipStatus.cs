using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // ������ ���������� ã�� ���� �߰�

public class ShipStatus : MonoBehaviour
{
    [Header("���� ���� �ܰ�")]
    public int currentStage = 2;

    [Header("�ܰ躰 �������� �� ���� (1~4�ܰ�)")]
    public GameObject[] repairingAppearances; // Broken Boat 1 ~ 4 ����

    [Header("���� �ϼ��� �� ���� (5�ܰ�)")]
    public GameObject completedShipAppearance; // Broken Boat 5 ����

    private int maxRepairStage;

    void Start()
    {
        if (ShipRepairDatabase.AllStages.Count > 0)
        {
            maxRepairStage = ShipRepairDatabase.AllStages.Keys.Max();
        }
        Debug.Log($"[ShipStatus.Start] Initial currentStage: {currentStage}");
        UpdateShipAppearance();
    }

    public void CompleteRepairStage()
    {
        if (currentStage >= maxRepairStage)
        {
            currentStage = maxRepairStage + 1;
            Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�!");
        }
        else
        {
            currentStage++;
            Debug.Log($"���� �Ϸ�! ���� �ܰ�: {currentStage}");
        }
        UpdateShipAppearance();
    }

    void UpdateShipAppearance()
    {
        foreach (var appearance in repairingAppearances)
        {
            if (appearance != null) appearance.SetActive(false);
        }
        if (completedShipAppearance != null) completedShipAppearance.SetActive(false);

        if (currentStage > maxRepairStage)
        {
            if (completedShipAppearance != null) completedShipAppearance.SetActive(true);
        }
        else
        {
            int appearanceIndex = currentStage - 2;
            if (appearanceIndex >= 0 && appearanceIndex < repairingAppearances.Length && repairingAppearances[appearanceIndex] != null)
            {
                repairingAppearances[appearanceIndex].SetActive(true);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 마지막 스테이지를 찾기 위해 추가

public class ShipStatus : MonoBehaviour
{
    [Header("현재 수리 단계")]
    public int currentStage = 2;

    [Header("단계별 수리중인 배 외형 (1~4단계)")]
    public GameObject[] repairingAppearances; // Broken Boat 1 ~ 4 연결

    [Header("최종 완성된 배 외형 (5단계)")]
    public GameObject completedShipAppearance; // Broken Boat 5 연결

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
            Debug.Log("모든 수리가 완료되었습니다!");
        }
        else
        {
            currentStage++;
            Debug.Log($"수리 완료! 다음 단계: {currentStage}");
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
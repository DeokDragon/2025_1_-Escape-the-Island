using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairInteraction : MonoBehaviour
{
    public GameObject ship; // 난파선 오브젝트
    private bool isNearShip = false; // 플레이어가 난파선 근처에 있는지 여부

    void Update()
    {
        if (isNearShip && Input.GetKeyDown(KeyCode.E))
        {
            // E 키가 눌리면 수리 시작
            RepairShip();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 근처에 들어오면 상호작용 가능
            isNearShip = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 떠나면 상호작용 불가능
            isNearShip = false;
        }
    }

    void RepairShip()
    {
        // 인벤토리에서 나무가 있는지 확인하고, 나무가 있으면 수리 진행
        if (Inventory.instance.HasItem("Log", 1))
        {
            // 나무 1개 소모 후 수리 진행
            Inventory.instance.ConsumeItem("Log", 1);
            // 수리 진행 후 모델 업데이트 등 추가 작업
            Debug.Log("수리 진행 중... 현재 나무 개수: " + Inventory.instance.HasItem("Log", 0));
        }
        else
        {
            Debug.Log("나무가 부족합니다.");
        }
    }
}
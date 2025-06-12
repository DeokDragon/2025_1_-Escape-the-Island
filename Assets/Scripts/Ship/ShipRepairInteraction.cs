using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairInteraction : MonoBehaviour
{
    public GameObject interactPromptUI;      // "E" 키 프롬프트 UI
    public Transform player;                 // 플레이어 Transform
    public float interactDistance = 3f;      // 상호작용 거리

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    // 이 메서드들은 비워두거나, 원래처럼 더미로 두면 됩니다.
    // ShipRepairUIController가 UI를 열고 닫는 실제 로직을 가지고 있습니다.
    public void OpenUI() { }
    public void CloseUI() { }

    [SerializeField] private ShipRepairUIController shipRepairUIController;

    void Update()
    {
        // Player Transform이 할당되었는지 확인합니다.
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in ShipRepairInteraction!");
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        isPlayerNear = (dist <= interactDistance);


        if (interactPromptUI != null)
            interactPromptUI.SetActive(!isUIOpen && isPlayerNear);

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShipRepairUI();
        }

        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShipRepairUI();
        }
    }

    void ToggleShipRepairUI()
    {
        isUIOpen = !isUIOpen;

        if (shipRepairUIController == null)
        {
            Debug.LogError("[ShipRepairInteraction ERROR] shipRepairUIController reference is NULL! Please assign it in the Inspector.");
            return;
        }

        if (isUIOpen)
        {
            shipRepairUIController.OpenUI(); // 올바른 호출
            // Debug.Log("[ShipRepairInteraction] Calling OpenUI() on shipRepairUIController.");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            shipRepairUIController.CloseUI(); // 올바른 호출
            // Debug.Log("[ShipRepairInteraction] Calling CloseUI() on shipRepairUIController.");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
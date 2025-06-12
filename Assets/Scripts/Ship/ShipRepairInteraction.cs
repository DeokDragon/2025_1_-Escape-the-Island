using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairInteraction : MonoBehaviour
{
    public GameObject interactPromptUI;      // "E" Ű ������Ʈ UI
    public Transform player;                 // �÷��̾� Transform
    public float interactDistance = 3f;      // ��ȣ�ۿ� �Ÿ�

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    // �� �޼������ ����ΰų�, ����ó�� ���̷� �θ� �˴ϴ�.
    // ShipRepairUIController�� UI�� ���� �ݴ� ���� ������ ������ �ֽ��ϴ�.
    public void OpenUI() { }
    public void CloseUI() { }

    [SerializeField] private ShipRepairUIController shipRepairUIController;

    void Update()
    {
        // Player Transform�� �Ҵ�Ǿ����� Ȯ���մϴ�.
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
            shipRepairUIController.OpenUI(); // �ùٸ� ȣ��
            // Debug.Log("[ShipRepairInteraction] Calling OpenUI() on shipRepairUIController.");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            shipRepairUIController.CloseUI(); // �ùٸ� ȣ��
            // Debug.Log("[ShipRepairInteraction] Calling CloseUI() on shipRepairUIController.");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
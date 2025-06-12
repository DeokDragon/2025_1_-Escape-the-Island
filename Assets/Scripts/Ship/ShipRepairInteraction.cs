using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairInteraction : MonoBehaviour
{
    public GameObject interactPromptUI;
    public Transform player;
    public float interactDistance = 3f;

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    public void OpenUI() { }
    public void CloseUI() { }

    [SerializeField] private ShipRepairUIController shipRepairUIController;

    void Update()
    {
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
            shipRepairUIController.OpenUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            shipRepairUIController.CloseUI();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
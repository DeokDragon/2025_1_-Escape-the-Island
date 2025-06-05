using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator animator;
    public GameObject chestUI;
    public GameObject interactPromptUI; // "E" UI
    public float interactDistance = 3f;
    public Transform player;
    public LayerMask Box; // 상호작용 가능한 레이어

    private bool isUIOpen = false;

    void Update()
    {
        if (isUIOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseChestUI();
                GameManager.escHandledThisFrame = true;
            }

            interactPromptUI.SetActive(false); // UI 열려있으면 E키 UI는 꺼둠
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool isLookingAtThisChest = false;

        if (Physics.Raycast(ray, out hit, interactDistance, Box))
        {
            if (hit.transform == transform)
            {
                isLookingAtThisChest = true;
                interactPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenChest();
                }
            }
        }

        if (!isLookingAtThisChest)
        {
            interactPromptUI.SetActive(false);
        }

        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseChestUI();
            GameManager.escHandledThisFrame = true;
        }
    }

    void OpenChest()
    {
        animator.SetTrigger("ToggleChest"); // 열고 닫는 동일 트리거
        ShowChestUI();
        interactPromptUI.SetActive(false);

        GameManager.isChestUIOpen = true;
        GameManager.canPlayerRotate = false;
        GameManager.canPlayerMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowChestUI()
    {
        chestUI.SetActive(true);
        isUIOpen = true;

        Slot[] slots = chestUI.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            slot.UpdateSlotUI();
        }
    }

    public void CloseChestUI()
    {
        animator.SetTrigger("ToggleChest");
        chestUI.SetActive(false);
        isUIOpen = false;

        GameManager.isChestUIOpen = false;
        GameManager.canPlayerRotate = true;
        GameManager.canPlayerMove = true;

        StartCoroutine(LockCursorNextFrame());
    }

    private IEnumerator LockCursorNextFrame()
    {
        yield return null; // 다음 프레임까지 대기

        Cursor.lockState = CursorLockMode.None; // 중립화
        Cursor.lockState = CursorLockMode.Locked; // 다시 잠금
        Cursor.visible = false;
    }
}


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
        if (GameManager.escHandledThisFrame)
            return;

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
        animator.SetTrigger("ToggleChest");
        ShowChestUI();
        interactPromptUI.SetActive(false);

        GameManager.isChestUIOpen = true;
        GameManager.canPlayerRotate = false;
        GameManager.canPlayerMove = false;

        GameManager.UpdateCursorState(); // 커서 자동 처리
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
        StartCoroutine(DisableChestUIAfterAnimation());

        GameManager.isChestUIOpen = false;
        GameManager.canPlayerRotate = true;
        GameManager.canPlayerMove = true;
        GameManager.UpdateCursorState();
    }

    private IEnumerator DisableChestUIAfterAnimation()
    {
        // 애니메이션 길이만큼 기다림 (예: 0.5초)
        yield return new WaitForSeconds(0.5f);
        chestUI.SetActive(false);
        isUIOpen = false;
    }
}


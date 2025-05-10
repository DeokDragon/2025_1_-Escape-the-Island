using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator animator;
    public GameObject chestUI;
    public GameObject interactPromptUI; // "E키를 누르세요" UI
    public float interactDistance = 3f;
    public Transform player;

    private bool isOpened = false;
    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNear = distance <= interactDistance;

        // 상자 근처면 E키 안내 UI
        if (isPlayerNear && !isOpened)
        {
            interactPromptUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenChest();
            }
        }
        else
        {
            interactPromptUI.SetActive(false);
        }

        // ESC 키로 UI 닫기
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseChestUI();
        }
    }

    void OpenChest()
    {
        animator.SetTrigger("OpenChest");
        animator.SetTrigger("OpenKey");
        isOpened = true;
        StartCoroutine(ShowChestUIAfterAnimation());
        GameManager.canPlayerRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator ShowChestUIAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        chestUI.SetActive(true);
        isUIOpen = true;

        // 커서 보이게
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 아이템 UI 업데이트
        Slot[] slots = chestUI.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            slot.UpdateSlotUI();
        }
    }

    public void CloseChestUI()
    {
        chestUI.SetActive(false);
        isUIOpen = false;

        Time.timeScale = 1f;

        GameManager.canPlayerRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


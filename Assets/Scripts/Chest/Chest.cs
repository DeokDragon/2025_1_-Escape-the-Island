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

    private bool isOpened = false;
    private bool isUIOpen = false;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 정중앙
        RaycastHit hit;

        bool isLookingAtThisChest = false;

        if (Physics.Raycast(ray, out hit, interactDistance, Box))
        {
            if (hit.transform == transform && !isOpened)
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
            GameManager.escHandledThisFrame = true; // ESC 처리 완료 표시
        }

      
    }

    void OpenChest()
    {
        animator.SetTrigger("OpenChest");
        animator.SetTrigger("OpenKey");
        isOpened = true;

        GameManager.isChestUIOpen = true; // 애니메이션 기다리지 말고 지금 바로 true로 설정

        StartCoroutine(ShowChestUIAfterAnimation());
        //플레이어 행동 묶기
        GameManager.canPlayerRotate = false;
        GameManager.canPlayerMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator ShowChestUIAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        chestUI.SetActive(true);
        isUIOpen = true;

        GameManager.isChestUIOpen = true; //열려 있나?
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

        GameManager.isChestUIOpen = false; //닫을 때 false
        //플레이어 움직임 묶기
        GameManager.canPlayerRotate = true;
        GameManager.canPlayerMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


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
        if (isUIOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseChestUI();
                GameManager.escHandledThisFrame = true;
            }
            interactPromptUI.SetActive(false); // UI 열려있으면 E키 UI는 무조건 꺼둠
            return;
        }


        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 정중앙
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
                    if (!isOpened)
                    {
                        if (Inventory.instance.HasItem("Key", 1))
                        {
                            Inventory.instance.ConsumeItem("Key", 1); // 열쇠 1개 소비
                            OpenChest();
                        }
                        else
                        {
                            Debug.Log("열쇠가 필요합니다!");
                        }
                    }
                    else
                    {
                        // 이미 열린 상태에서 다시 UI 열기
                        OpenChest();
                    }
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
        if (!isOpened)
        {
            animator.SetTrigger("OpenChest");
            animator.SetTrigger("OpenKey");
            isOpened = true;

            StartCoroutine(ShowChestUIAfterAnimation());
        }
        else
        {
            // 이미 열려있으면 바로 UI 활성화
            ShowChestUIInstantly();
        }

        interactPromptUI.SetActive(false);

        GameManager.isChestUIOpen = true;

        GameManager.canPlayerRotate = false;
        GameManager.canPlayerMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowChestUIInstantly()
    {
        chestUI.SetActive(true);
        isUIOpen = true;

        Slot[] slots = chestUI.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            slot.UpdateSlotUI();
        }
    }

    private IEnumerator ShowChestUIAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        ShowChestUIInstantly();

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


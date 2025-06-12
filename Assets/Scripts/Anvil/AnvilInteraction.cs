using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilInteraction : MonoBehaviour
{
    public GameObject interactPromptUI;     // "E" 키 프롬프트 UI
    public Transform player;                // 플레이어 Transform
    public float interactDistance = 3f;     // 상호작용 거리

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    [SerializeField] private GameObject anvilUI; // 강화 UI 오브젝트

    void Update()
    {
        // 1. 플레이어와의 거리를 먼저 확인
        float dist = Vector3.Distance(player.position, transform.position);
        isPlayerNear = (dist <= interactDistance);

        // UI가 닫혀 있고 플레이어가 근처에 있을 때만 "E키" 프롬프트를 표시
        interactPromptUI.SetActive(!isUIOpen && isPlayerNear);

        // 2. 'E' 키 입력 처리 (토글 기능)
        // 플레이어가 근처에 있을 때 'E'키를 누르면
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            // isUIOpen 값에 따라 열거나 닫는 함수를 호출
            if (!isUIOpen)
            {
                OpenAnvilUI();
            }
            else
            {
                CloseAnvilUI();
            }
        }

        // 3. 'ESC' 키 입력 처리 (기존 기능 유지)
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape) && !GameManager.escHandledThisFrame)
        {
            CloseAnvilUI();
            GameManager.escHandledThisFrame = true;
        }
    }

    void CheckPlayerNear()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactDistance)
        {
            isPlayerNear = true;
            interactPromptUI.SetActive(true);
        }
        else
        {
            isPlayerNear = false;
            interactPromptUI.SetActive(false);
        }
    }

    void TryInteractInput()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OpenAnvilUI();
        }
    }

    void OpenAnvilUI()
    {
        anvilUI.SetActive(true);
        
        anvilUI.GetComponent<AnvilUIController>().RefreshPreview();

        isUIOpen = true;

        GameManager.isAnvilUIOpen = true;
        GameManager.UpdateCursorState();
    }

    void CloseAnvilUI()
    {
        anvilUI.SetActive(false);
        isUIOpen = false;

        GameManager.isAnvilUIOpen = false;
        GameManager.UpdateCursorState();

        interactPromptUI.SetActive(false);
    }
}

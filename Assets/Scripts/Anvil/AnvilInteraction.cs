using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilInteraction : MonoBehaviour
{
    public GameObject interactPromptUI;     // "E" Ű ������Ʈ UI
    public Transform player;                // �÷��̾� Transform
    public float interactDistance = 3f;     // ��ȣ�ۿ� �Ÿ�

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    [SerializeField] private GameObject anvilUI; // ��ȭ UI ������Ʈ

    void Update()
    {
        // 1. �÷��̾���� �Ÿ��� ���� Ȯ��
        float dist = Vector3.Distance(player.position, transform.position);
        isPlayerNear = (dist <= interactDistance);

        // UI�� ���� �ְ� �÷��̾ ��ó�� ���� ���� "EŰ" ������Ʈ�� ǥ��
        interactPromptUI.SetActive(!isUIOpen && isPlayerNear);

        // 2. 'E' Ű �Է� ó�� (��� ���)
        // �÷��̾ ��ó�� ���� �� 'E'Ű�� ������
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            // isUIOpen ���� ���� ���ų� �ݴ� �Լ��� ȣ��
            if (!isUIOpen)
            {
                OpenAnvilUI();
            }
            else
            {
                CloseAnvilUI();
            }
        }

        // 3. 'ESC' Ű �Է� ó�� (���� ��� ����)
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

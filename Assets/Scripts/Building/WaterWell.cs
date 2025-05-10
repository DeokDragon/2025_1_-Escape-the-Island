using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWell : MonoBehaviour
{
    public GameObject interactPromptUI; // EŰ UI ǥ�ÿ�
    public LayerMask playerLayer; // �÷��̾� ���̾�
    public float interactDistance = 3f; // ��ȣ�ۿ� �Ÿ�
    public int thirstIncreaseAmount = 20; // ���� ������ �� �����ϴ� �񸶸� ��

    private bool isPlayerNearby = false; // �÷��̾ �칰 ��ó�� �ִ��� Ȯ��
    private StatusController playerStatusController; // �÷��̾� ���� ��Ʈ�ѷ�

    void Start()
    {
        // �÷��̾� ���� ��Ʈ�ѷ� ã��
        playerStatusController = FindObjectOfType<StatusController>();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // �÷��̾��� �ü� �������� Ray�� ���� �칰 ����

        if (Physics.Raycast(ray, out hit, interactDistance, playerLayer))
        {
            if (hit.transform.CompareTag("WaterWell")) // �칰 �±׿� ��
            {
                isPlayerNearby = true;
                interactPromptUI.SetActive(true); // EŰ UI ǥ��

                if (Input.GetKeyDown(KeyCode.E)) // E Ű�� ������ ��
                {
                    DrinkWater();
                }
            }
        }
        else
        {
            isPlayerNearby = false;
            interactPromptUI.SetActive(false); // EŰ UI �����
        }
    }

    void DrinkWater()
    {
        if (playerStatusController != null)
        {
            playerStatusController.IncreaseThirsty(thirstIncreaseAmount);
        }
    }
}

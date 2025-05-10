using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWell : MonoBehaviour
{
    public GameObject interactPromptUI; // E키 UI 표시용
    public LayerMask playerLayer; // 플레이어 레이어
    public float interactDistance = 3f; // 상호작용 거리
    public int thirstIncreaseAmount = 20; // 물을 마셨을 때 증가하는 목마름 양

    private bool isPlayerNearby = false; // 플레이어가 우물 근처에 있는지 확인
    private StatusController playerStatusController; // 플레이어 상태 컨트롤러

    void Start()
    {
        // 플레이어 상태 컨트롤러 찾기
        playerStatusController = FindObjectOfType<StatusController>();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // 플레이어의 시선 방향으로 Ray를 쏴서 우물 감지

        if (Physics.Raycast(ray, out hit, interactDistance, playerLayer))
        {
            if (hit.transform.CompareTag("WaterWell")) // 우물 태그와 비교
            {
                isPlayerNearby = true;
                interactPromptUI.SetActive(true); // E키 UI 표시

                if (Input.GetKeyDown(KeyCode.E)) // E 키를 눌렀을 때
                {
                    DrinkWater();
                }
            }
        }
        else
        {
            isPlayerNearby = false;
            interactPromptUI.SetActive(false); // E키 UI 숨기기
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

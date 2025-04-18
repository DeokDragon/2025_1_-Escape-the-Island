using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WreckShipRepair : MonoBehaviour
{

    public GameObject[] shipModelStages;
    public int woodRequired = 40;
    public string requiredItemName = "Log"; // "Log" 아이템
    private int currentWood = 0;

    private bool isPlayerInRange = false;
    private bool canRepair = true;

    private float lastRepairTime = 0f;
    private float repairCooldown = 0.2f;

    // 부모 객체의 Transform을 이용하여 위치와 회전을 관리
    private Vector3 basePosition;
    private Quaternion baseRotation;

    public AudioClip repairSound; // 수리 사운드
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Start()
    {
        // 부모 객체의 기본 위치와 회전값을 설정
        basePosition = transform.position;
        baseRotation = transform.rotation;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && canRepair)
        {
            StartCoroutine(RepairShipCoroutine());
        }
    }

    private IEnumerator RepairShipCoroutine()
    {
        canRepair = false;

        if (currentWood >= woodRequired)
        {
            Debug.Log("난파선 이미 수리 완료");
            canRepair = true;
            yield break;
        }

        // 인벤토리에서 나무가 충분히 있는지 체크
        if (Inventory.instance.HasItem(requiredItemName, 1))
        {
            Inventory.instance.ConsumeItem(requiredItemName, 1);
            currentWood++;
            UpdateShipModel();
            Debug.Log("나무 추가됨! 현재 나무: " + currentWood);

            // 사운드 재생
            if (repairSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(repairSound);
            }
        }
        else
        {
            Debug.Log("나무 부족! 수리가 진행되지 않았습니다.");
        }

        yield return new WaitForSeconds(0.3f); // 0.3초 딜레이 추가
        canRepair = true;
    }

    private void UpdateShipModel()
    {
        int stageCount = shipModelStages.Length - 1; // 스테이지의 총 개수
        int stage = Mathf.Clamp(currentWood * stageCount / woodRequired, 0, stageCount); // 나무 수에 따라 스테이지 계산

        Debug.Log($"[UpdateShipModel] 현재 나무: {currentWood}, 계산된 스테이지: {stage}");

        for (int i = 0; i < shipModelStages.Length; i++)
        {
            bool isActive = i == stage; // 해당 스테이지의 모델만 활성화
            shipModelStages[i].SetActive(isActive); // 활성화/비활성화 처리

            Debug.Log($"→ {shipModelStages[i].name} {(isActive ? "활성화" : "비활성화")}");

            // 활성화된 모델의 위치와 회전값을 기준에 맞추기
            if (isActive)
            {
                Vector3 basePosition = shipModelStages[0].transform.position;
                Quaternion baseRotation = shipModelStages[0].transform.rotation;
                shipModelStages[i].transform.position = basePosition;
                shipModelStages[i].transform.rotation = baseRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("플레이어 난파선 범위 진입");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("플레이어 난파선 범위 이탈");
        }
    }
}
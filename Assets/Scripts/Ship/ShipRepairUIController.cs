using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipRepairUIController : MonoBehaviour
{
    [Header("UI 오브젝트")]
    [SerializeField] private GameObject go_ShipRepairUIBase;      // 기존 반투명 UI 배경 (이것만 남김)
    [SerializeField] private Slot[] materialSlots;                // 플레이어가 재료를 올리는 슬롯
    [SerializeField] private TextMeshProUGUI stageInfoText;       // 기존 단계 정보 텍스트
    [SerializeField] private Button repairButton;                 // 수리 버튼

    [SerializeField] private Slot[] requiredMaterialSlots;        // 필요 재료를 보여주는 슬롯

    [Header("상태 스크립트")]
    [SerializeField] private ShipStatus shipStatus;

    [Header("오디오 설정")]
    [SerializeField] private AudioSource repairAudioSource;
    [SerializeField] private AudioClip repairSoundClip;

    // "수리 완료!" 메시지 텍스트 (기존 UI 배경 안에 위치)
    [SerializeField] private TextMeshProUGUI repairCompletedMessageText; // 변수 이름 변경

    // 추가: 기존 UI에 표시되는 다른 텍스트 요소들
    [Header("기존 UI 기타 텍스트")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI requiredMaterialsLabel;
    [SerializeField] private TextMeshProUGUI insertMaterialsPrompt;
    [SerializeField] private TextMeshProUGUI closePromptText;
    


    private float uiCloseTimer = 0f;
    private bool repairCompletedSequenceActive = false; // 수리 완료 후 시퀀스가 진행 중인지 나타내는 플래그


    void Start()
    {
        if (repairButton != null)
        {
            repairButton.onClick.AddListener(OnRepairButtonClick);
        }

        if (repairAudioSource == null)
        {
            repairAudioSource = GetComponent<AudioSource>();
            if (repairAudioSource == null)
            {
                Debug.LogWarning("ShipRepairUIController: AudioSource가 연결되지 않았으며, 이 GameObject에서 찾을 수 없습니다.");
            }
        }
        // repairCompletedMessageText는 처음에는 숨겨져 있어야 함
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
    }

    public void OpenUI()
    {
        go_ShipRepairUIBase.SetActive(true);
        // UI가 열릴 때 수리 완료 메시지는 항상 숨깁니다.
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
        // 다른 모든 일반 UI 요소들이 정상적으로 보이도록 설정합니다.
        SetRepairUIElementsActive(true); // 새로 추가할 헬퍼 메서드 호출

        RefreshUI(); // 현재 단계에 맞는 UI를 표시하기 위해 호출
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 수리 완료 시퀀스 플래그 초기화
        repairCompletedSequenceActive = false;
    }

    public void CloseUI()
    {
        go_ShipRepairUIBase.SetActive(false);
        // 모든 UI 요소들을 비활성화하여 혹시 모를 잔상을 방지합니다.
        SetRepairUIElementsActive(false); // 헬퍼 메서드 호출
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        repairCompletedSequenceActive = false; // 닫힐 때 시퀀스 플래그 초기화
    }

    public void RefreshUI()
    {
        // 1. 기존 정보 초기화 (기존 슬롯들 비우기)
        foreach (var slot in requiredMaterialSlots)
        {
            slot.ClearSlot();
        }

        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage);

        if (currentStageInfo == null)
        {
            // 모든 수리가 완료된 경우
            go_ShipRepairUIBase.SetActive(true); // UI 배경은 유지

            SetRepairUIElementsActive(false); // 모든 일반 UI 요소 숨기기

            if (repairCompletedMessageText != null)
            {
                repairCompletedMessageText.text = "수리 완료!";
                repairCompletedMessageText.gameObject.SetActive(true); // "수리 완료!" 텍스트 활성화
            }
            else
            {
                Debug.LogWarning("ShipRepairUIController: repairCompletedMessageText가 할당되지 않았습니다. '수리 완료!' 메시지를 표시할 수 없습니다.");
            }

            repairCompletedSequenceActive = true; // 수리 완료 시퀀스 시작 플래그
            uiCloseTimer = 5f; // 5초 타이머 시작
            return; // 여기서 리턴하여 아래의 재료 표시 로직을 건너뜁니다.
        }

        // 수리가 완료되지 않은 일반적인 단계일 때
        go_ShipRepairUIBase.SetActive(true); // UI 배경은 항상 활성화
        SetRepairUIElementsActive(true); // 일반 UI 요소들 다시 활성화

        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false); // 수리 완료 메시지 숨기기
        }

        stageInfoText.text = $"수리 단계: {currentStageInfo.stageName}";

        // 3. 필요 재료를 슬롯에 아이콘으로 표시하기 (기존 로직 그대로)
        int i = 0;
        foreach (var req in currentStageInfo.requiredMaterials)
        {
            if (i >= requiredMaterialSlots.Length) break;

            string materialName = req.Key;
            int materialAmount = req.Value;
            Item materialItem = Resources.Load<Item>("Item/" + materialName);

            if (materialItem != null && requiredMaterialSlots[i] != null)
            {
                requiredMaterialSlots[i].ForceAddItem(materialItem, materialAmount);
            }
            else
            {
                if (materialItem == null) Debug.LogError($"Failed to load material Item for '{materialName}'. Check Resources/Item path and item name.");
                if (requiredMaterialSlots[i] == null) Debug.LogError($"RequiredMaterialSlot at index {i} is null in Inspector.");
            }
            i++;
        }
    }

    bool HasEnoughMaterials(ShipRepairStage stage)
    {
        foreach (var req in stage.requiredMaterials)
        {
            int totalInSlots = 0;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    totalInSlots += slot.itemCount;
                }
            }
            if (totalInSlots < req.Value) return false;
        }
        return true;
    }

    void ConsumeMaterials(ShipRepairStage stage)
    {
        foreach (var req in stage.requiredMaterials)
        {
            int remaining = req.Value;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    int consume = Mathf.Min(slot.itemCount, remaining);
                    slot.SetSlotCount(-consume);
                    remaining -= consume;
                    if (remaining <= 0) break;
                }
            }
        }
    }

    public void OnRepairButtonClick() // 이 메서드를 실제 코드에 반영합니다.
    {
        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage); //
        if (currentStageInfo == null) return; //

        if (HasEnoughMaterials(currentStageInfo)) //
        {
            ConsumeMaterials(currentStageInfo); //
            PlayRepairSound(); // 소리 재생 호출 위치 변경
            shipStatus.CompleteRepairStage(); //
            RefreshUI(); //
        }
        else
        {
            Debug.Log("재료가 부족합니다."); //
        }
    }

    // 선박 수리 사운드를 재생하는 새 메서드
    private void PlayRepairSound()
    {
        if (repairAudioSource != null && repairSoundClip != null)
        {
            repairAudioSource.PlayOneShot(repairSoundClip); // 한 번만 재생
            Debug.Log("Repair sound played.");
        }
        else if (repairAudioSource == null)
        {
            Debug.LogWarning("Repair AudioSource is not assigned in ShipRepairUIController.");
        }
        else if (repairSoundClip == null)
        {
            Debug.LogWarning("Repair Sound Clip is not assigned in ShipRepairUIController.");
        }
    }

    void Update()
    {
        if (repairCompletedSequenceActive)
        {
            uiCloseTimer -= Time.deltaTime; // 초당 시간 감소
            if (uiCloseTimer <= 0f)
            {
                CloseUI(); // UI 닫기
                repairCompletedSequenceActive = false; // 시퀀스 종료 플래그
            }
        }
    }
    private void SetRepairUIElementsActive(bool isActive)
    {
        if (stageInfoText != null) stageInfoText.gameObject.SetActive(isActive);
        if (repairButton != null) repairButton.gameObject.SetActive(isActive);

        foreach (var slot in materialSlots)
        {
            if (slot != null && slot.gameObject != null) slot.gameObject.SetActive(isActive);
        }
        foreach (var slot in requiredMaterialSlots)
        {
            if (slot != null && slot.gameObject != null) slot.gameObject.SetActive(isActive);
        }

        // 새로 추가된 텍스트 요소들 제어
        if (titleText != null) titleText.gameObject.SetActive(isActive);
        if (requiredMaterialsLabel != null) requiredMaterialsLabel.gameObject.SetActive(isActive);
        if (insertMaterialsPrompt != null) insertMaterialsPrompt.gameObject.SetActive(isActive);
        if (closePromptText != null) closePromptText.gameObject.SetActive(isActive);
        

        // 이미지에서 보이는 다른 모든 UI 요소들이 있다면 여기에 추가하여 제어합니다.
        // 예를 들어, 재료 슬롯 아래 "재료를 넣으십시오"라는 텍스트가 있다면 해당 TextMeshProUGUI를 참조하여 추가합니다.
    }
}
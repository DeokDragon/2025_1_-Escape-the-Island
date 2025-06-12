using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipRepairUIController : MonoBehaviour
{
    [Header("UI 오브젝트")]
    [SerializeField] private GameObject go_ShipRepairUIBase;
    [SerializeField] private Slot[] materialSlots;
    [SerializeField] private TextMeshProUGUI stageInfoText;
    [SerializeField] private Button repairButton;

    [SerializeField] private Slot[] requiredMaterialSlots;

    [Header("상태 스크립트")]
    [SerializeField] private ShipStatus shipStatus;

    [Header("오디오 설정")]
    [SerializeField] private AudioSource repairAudioSource;
    [SerializeField] private AudioClip repairSoundClip;

    [SerializeField] private TextMeshProUGUI repairCompletedMessageText;

    [Header("기존 UI 기타 텍스트")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI requiredMaterialsLabel;
    [SerializeField] private TextMeshProUGUI insertMaterialsPrompt;
    [SerializeField] private TextMeshProUGUI closePromptText;

    private float uiCloseTimer = 0f;
    private bool repairCompletedSequenceActive = false;

    void Start()
    {
        if (repairButton != null)
        {
            repairButton.onClick.AddListener(OnRepairButtonClick);
        }

        if (repairAudioSource == null)
        {
            repairAudioSource = GetComponent<AudioSource>();
        }
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
    }

    public void OpenUI()
    {
        go_ShipRepairUIBase.SetActive(true);
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
        SetRepairUIElementsActive(true);

        RefreshUI();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        repairCompletedSequenceActive = false;
    }

    public void CloseUI()
    {
        go_ShipRepairUIBase.SetActive(false);
        SetRepairUIElementsActive(false);
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        repairCompletedSequenceActive = false;
    }

    public void RefreshUI()
    {
        foreach (var slot in requiredMaterialSlots)
        {
            slot.ClearSlot();
        }

        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage);

        if (currentStageInfo == null)
        {
            go_ShipRepairUIBase.SetActive(true);

            SetRepairUIElementsActive(false);

            if (repairCompletedMessageText != null)
            {
                repairCompletedMessageText.text = "수리 완료!";
                repairCompletedMessageText.gameObject.SetActive(true);
            }

            repairCompletedSequenceActive = true;
            uiCloseTimer = 5f;
            return;
        }

        go_ShipRepairUIBase.SetActive(true);
        SetRepairUIElementsActive(true);

        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }

        stageInfoText.text = $"수리 단계: {currentStageInfo.stageName}";

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

    public void OnRepairButtonClick()
    {
        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage);
        if (currentStageInfo == null) return;

        if (HasEnoughMaterials(currentStageInfo))
        {
            ConsumeMaterials(currentStageInfo);
            PlayRepairSound();
            shipStatus.CompleteRepairStage();
            RefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    private void PlayRepairSound()
    {
        if (repairAudioSource != null && repairSoundClip != null)
        {
            repairAudioSource.PlayOneShot(repairSoundClip);
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
            uiCloseTimer -= Time.deltaTime;
            if (uiCloseTimer <= 0f)
            {
                CloseUI();
                repairCompletedSequenceActive = false;
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

        if (titleText != null) titleText.gameObject.SetActive(isActive);
        if (requiredMaterialsLabel != null) requiredMaterialsLabel.gameObject.SetActive(isActive);
        if (insertMaterialsPrompt != null) insertMaterialsPrompt.gameObject.SetActive(isActive);
        if (closePromptText != null) closePromptText.gameObject.SetActive(isActive);
    }
}
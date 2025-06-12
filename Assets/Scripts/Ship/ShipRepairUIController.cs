using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipRepairUIController : MonoBehaviour
{
    [Header("UI ������Ʈ")]
    [SerializeField] private GameObject go_ShipRepairUIBase;      // ���� ������ UI ��� (�̰͸� ����)
    [SerializeField] private Slot[] materialSlots;                // �÷��̾ ��Ḧ �ø��� ����
    [SerializeField] private TextMeshProUGUI stageInfoText;       // ���� �ܰ� ���� �ؽ�Ʈ
    [SerializeField] private Button repairButton;                 // ���� ��ư

    [SerializeField] private Slot[] requiredMaterialSlots;        // �ʿ� ��Ḧ �����ִ� ����

    [Header("���� ��ũ��Ʈ")]
    [SerializeField] private ShipStatus shipStatus;

    [Header("����� ����")]
    [SerializeField] private AudioSource repairAudioSource;
    [SerializeField] private AudioClip repairSoundClip;

    // "���� �Ϸ�!" �޽��� �ؽ�Ʈ (���� UI ��� �ȿ� ��ġ)
    [SerializeField] private TextMeshProUGUI repairCompletedMessageText; // ���� �̸� ����

    // �߰�: ���� UI�� ǥ�õǴ� �ٸ� �ؽ�Ʈ ��ҵ�
    [Header("���� UI ��Ÿ �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI requiredMaterialsLabel;
    [SerializeField] private TextMeshProUGUI insertMaterialsPrompt;
    [SerializeField] private TextMeshProUGUI closePromptText;
    


    private float uiCloseTimer = 0f;
    private bool repairCompletedSequenceActive = false; // ���� �Ϸ� �� �������� ���� ������ ��Ÿ���� �÷���


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
                Debug.LogWarning("ShipRepairUIController: AudioSource�� ������� �ʾ�����, �� GameObject���� ã�� �� �����ϴ�.");
            }
        }
        // repairCompletedMessageText�� ó������ ������ �־�� ��
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
    }

    public void OpenUI()
    {
        go_ShipRepairUIBase.SetActive(true);
        // UI�� ���� �� ���� �Ϸ� �޽����� �׻� ����ϴ�.
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }
        // �ٸ� ��� �Ϲ� UI ��ҵ��� ���������� ���̵��� �����մϴ�.
        SetRepairUIElementsActive(true); // ���� �߰��� ���� �޼��� ȣ��

        RefreshUI(); // ���� �ܰ迡 �´� UI�� ǥ���ϱ� ���� ȣ��
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ���� �Ϸ� ������ �÷��� �ʱ�ȭ
        repairCompletedSequenceActive = false;
    }

    public void CloseUI()
    {
        go_ShipRepairUIBase.SetActive(false);
        // ��� UI ��ҵ��� ��Ȱ��ȭ�Ͽ� Ȥ�� �� �ܻ��� �����մϴ�.
        SetRepairUIElementsActive(false); // ���� �޼��� ȣ��
        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        repairCompletedSequenceActive = false; // ���� �� ������ �÷��� �ʱ�ȭ
    }

    public void RefreshUI()
    {
        // 1. ���� ���� �ʱ�ȭ (���� ���Ե� ����)
        foreach (var slot in requiredMaterialSlots)
        {
            slot.ClearSlot();
        }

        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage);

        if (currentStageInfo == null)
        {
            // ��� ������ �Ϸ�� ���
            go_ShipRepairUIBase.SetActive(true); // UI ����� ����

            SetRepairUIElementsActive(false); // ��� �Ϲ� UI ��� �����

            if (repairCompletedMessageText != null)
            {
                repairCompletedMessageText.text = "���� �Ϸ�!";
                repairCompletedMessageText.gameObject.SetActive(true); // "���� �Ϸ�!" �ؽ�Ʈ Ȱ��ȭ
            }
            else
            {
                Debug.LogWarning("ShipRepairUIController: repairCompletedMessageText�� �Ҵ���� �ʾҽ��ϴ�. '���� �Ϸ�!' �޽����� ǥ���� �� �����ϴ�.");
            }

            repairCompletedSequenceActive = true; // ���� �Ϸ� ������ ���� �÷���
            uiCloseTimer = 5f; // 5�� Ÿ�̸� ����
            return; // ���⼭ �����Ͽ� �Ʒ��� ��� ǥ�� ������ �ǳʶݴϴ�.
        }

        // ������ �Ϸ���� ���� �Ϲ����� �ܰ��� ��
        go_ShipRepairUIBase.SetActive(true); // UI ����� �׻� Ȱ��ȭ
        SetRepairUIElementsActive(true); // �Ϲ� UI ��ҵ� �ٽ� Ȱ��ȭ

        if (repairCompletedMessageText != null)
        {
            repairCompletedMessageText.gameObject.SetActive(false); // ���� �Ϸ� �޽��� �����
        }

        stageInfoText.text = $"���� �ܰ�: {currentStageInfo.stageName}";

        // 3. �ʿ� ��Ḧ ���Կ� ���������� ǥ���ϱ� (���� ���� �״��)
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

    public void OnRepairButtonClick() // �� �޼��带 ���� �ڵ忡 �ݿ��մϴ�.
    {
        ShipRepairStage currentStageInfo = ShipRepairDatabase.GetStageInfo(shipStatus.currentStage); //
        if (currentStageInfo == null) return; //

        if (HasEnoughMaterials(currentStageInfo)) //
        {
            ConsumeMaterials(currentStageInfo); //
            PlayRepairSound(); // �Ҹ� ��� ȣ�� ��ġ ����
            shipStatus.CompleteRepairStage(); //
            RefreshUI(); //
        }
        else
        {
            Debug.Log("��ᰡ �����մϴ�."); //
        }
    }

    // ���� ���� ���带 ����ϴ� �� �޼���
    private void PlayRepairSound()
    {
        if (repairAudioSource != null && repairSoundClip != null)
        {
            repairAudioSource.PlayOneShot(repairSoundClip); // �� ���� ���
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
            uiCloseTimer -= Time.deltaTime; // �ʴ� �ð� ����
            if (uiCloseTimer <= 0f)
            {
                CloseUI(); // UI �ݱ�
                repairCompletedSequenceActive = false; // ������ ���� �÷���
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

        // ���� �߰��� �ؽ�Ʈ ��ҵ� ����
        if (titleText != null) titleText.gameObject.SetActive(isActive);
        if (requiredMaterialsLabel != null) requiredMaterialsLabel.gameObject.SetActive(isActive);
        if (insertMaterialsPrompt != null) insertMaterialsPrompt.gameObject.SetActive(isActive);
        if (closePromptText != null) closePromptText.gameObject.SetActive(isActive);
        

        // �̹������� ���̴� �ٸ� ��� UI ��ҵ��� �ִٸ� ���⿡ �߰��Ͽ� �����մϴ�.
        // ���� ���, ��� ���� �Ʒ� "��Ḧ �����ʽÿ�"��� �ؽ�Ʈ�� �ִٸ� �ش� TextMeshProUGUI�� �����Ͽ� �߰��մϴ�.
    }
}
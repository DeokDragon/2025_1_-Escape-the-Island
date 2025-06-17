using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CraftType { Placeable, Armor, Material }

[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab;
    public string[] craftNeedItem;
    public int[] craftNeedItemCount;
    public GameObject go_PreviewPrefab;

    public CraftType craftType; 
    public string itemID; // Material 타입용 (아이템 DB에서 사용될 이름)

    [Header("제작 수량 (슬롯별 설정 가능)")]
    public int craftAmount = 1; 
}

public class CraftManual : MonoBehaviour
    {
        private Inventory inventory;

        //상태변수
        private bool isActivated = false;
        private bool isPreviewActivated = false;

    //사운드 
    [SerializeField] private AudioSource audioSource;   // 사운드 재생용 AudioSource
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip buildSound;
    [SerializeField] private AudioClip OpenSound;

    public bool IsUIActivated()
    {
        return isActivated;
    }

    public bool IsPreviewMode()
    {
        return isPreviewActivated;
    }

    [SerializeField]
        private GameObject go_BaseUI; // 기본 베이스 UI

        private int selectedSlotIndex = -1;


        [SerializeField]
        private Craft[] craft_fire; // 모닥불용 탭.

        private GameObject go_Preview; // 미리보기 프리팹을 담을 변수.
        private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수.

        [SerializeField]
        private Transform tf_Player; // 플레이어 위치

        // Raycast 필요 변수 선언
        private RaycastHit hitInfo;
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private float range;

        //필요UI
        [SerializeField]
        private Text[] text_SlotNeedItem;
        //회전 돌리는거 qe전용
       private Quaternion targetRotation; // 목표 회전값
      [SerializeField] private float rotateSpeed = 180f; // 초당 회전 속도 (degree/second



    //armor 관련 함수
    [SerializeField] private GameObject craftUI_armor; // 노란 버튼 탭 (Armor)
    private enum CraftTab { Craft, Armor }
    [SerializeField] private Craft[] craft_armor;
    [SerializeField] private GameObject helmetIcon;
    [SerializeField] private GameObject armorIcon;
    [SerializeField] private GameObject pantsIcon;
    [SerializeField] private GameObject bootsIcon;

    [SerializeField] private float helmetColdProtection = 0.1f;
    [SerializeField] private float armorColdProtection = 0.25f;
    [SerializeField] private float pantsColdProtection = 0.2f;
    [SerializeField] private float bootsColdProtection = 0.15f;

    private bool isHelmetEquipped = false;
    private bool isArmorEquipped = false;
    private bool isPantsEquipped = false;
    private bool isBootsEquipped = false;

    public void SetHelmetEquipped(bool equipped) => isHelmetEquipped = equipped;
    public void SetArmorEquipped(bool equipped) => isArmorEquipped = equipped;
    public void SetPantsEquipped(bool equipped) => isPantsEquipped = equipped;
    public void SetBootsEquipped(bool equipped) => isBootsEquipped = equipped;


    //crafttab ui 칸
    [SerializeField] private GameObject craftUI1;
    [SerializeField] private GameObject craftUI2;
    [SerializeField] private GameObject craftUI3;

    private int currentPage = 1;

    public void SlotClick(int _slotNumber)
    {
        PlayClickSound();

        selectedSlotIndex = _slotNumber;
        Craft selectedCraft = craft_fire[_slotNumber];

        switch (selectedCraft.craftType)
        {
            case CraftType.Placeable:
                if (!CheckMaterials(_slotNumber))
                {
                    Debug.Log("자재가 부족하여 제작할 수 없습니다!");
                    return;
                }

                go_Preview = Instantiate(selectedCraft.go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
                go_Prefab = selectedCraft.go_Prefab;
                isPreviewActivated = true;
                targetRotation = go_Preview.transform.rotation;

                go_BaseUI.SetActive(false);
                GameManager.isCraftManualOpen = false;
                GameManager.UpdateCursorState(); // 커서 및 이동 상태 자동 설정
                break;

            case CraftType.Material:
                BuildMaterial();
                break;
        }
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>(); // 인벤토리 자동 검색

        // 초기 탭 설정
        craftUI1.SetActive(true);
        craftUI2.SetActive(false);
        craftUI3.SetActive(false);
        craftUI_armor.SetActive(false);

        //아이콘 초기화 (모두 꺼놓기)
        helmetIcon.SetActive(false);
        armorIcon.SetActive(false);
        pantsIcon.SetActive(false);
        bootsIcon.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        if (isPreviewActivated)
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1")) // 왼쪽 클릭: 설치
            Build();

        if (Input.GetMouseButtonDown(1)) // 👉 오른쪽 클릭: 취소
            Cancel();
    }


    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            int selectedSlot = GetSelectedSlot();

            if (!CheckMaterials(selectedSlot))
            {
                Debug.Log("재료가 부족하여 제작할 수 없습니다!");
                return;
            }

            PlayBuildSound();
            ConsumeMaterials(selectedSlot);

            // 이 부분 수정됨:
            GameObject placedObject = Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
            SaveManager.instance.CurrentSaveData.spawnedObjects.Add(new SpawnedObjectData
            {
                prefabName = go_Prefab.name,
                position = placedObject.transform.position,
                rotation = placedObject.transform.eulerAngles
            });



            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }


    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                float rotateSpeed = 90f; // 초당 90도 회전

                // Q, E 누르고 있는 동안 천천히 회전
                if (Input.GetKey(KeyCode.Q))
                    go_Preview.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0f);
                else if (Input.GetKey(KeyCode.E))
                    go_Preview.transform.Rotate(0, +rotateSpeed * Time.deltaTime, 0f);

                go_Preview.transform.position = _location;
            }
        }
    }

    private void Cancel()
        {
            if (isPreviewActivated)
                Destroy(go_Preview);

            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;



            go_BaseUI.SetActive(false);
        }

    public void CancelCraft()
    {
        if (isPreviewActivated)
            Cancel();
    }
    private void Window()
        {
            if (!isActivated)
                OpenWindow();
            else
                CloseWindow();
        }

    private void OpenWindow()
    {
        PlayOpenSound();
        isActivated = true;
        go_BaseUI.SetActive(true);

        GameManager.isCraftManualOpen = true;
        GameManager.UpdateCursorState();
    }

    private void CloseWindow()
    {
        PlayOpenSound();
        isActivated = false;
        go_BaseUI.SetActive(false);

        GameManager.isCraftManualOpen = false;
        GameManager.UpdateCursorState();
    }

    private bool CheckMaterials(int _slotNumber)
        {
            Craft craft = craft_fire[_slotNumber];

            for (int i = 0; i < craft.craftNeedItem.Length; i++)
            {
                if (!inventory.HasItem(craft.craftNeedItem[i], craft.craftNeedItemCount[i]))
                {
                    Debug.Log("재료 부족: " + craft.craftNeedItem[i]);
                    return false;  // 재료가 부족하면 제작 불가능
                }
            }
            return true;
        }

        private void ConsumeMaterials(int _slotNumber)
        {
            Craft craft = craft_fire[_slotNumber];

            for (int i = 0; i < craft.craftNeedItem.Length; i++)
            {
                inventory.ConsumeItem(craft.craftNeedItem[i], craft.craftNeedItemCount[i]);
            }
        }

        private int GetSelectedSlot()
        {
            return selectedSlotIndex;
        }

        public void OnCloseButtonClick()
        {
            CloseWindow();
        }

    public void NextPage()
    {
        PlayClickSound();

        if (currentPage == 1)
        {
            craftUI1.SetActive(false);
            craftUI2.SetActive(true);
            currentPage = 2;
        }
        else if (currentPage == 2)
        {
            craftUI2.SetActive(false);
            craftUI3.SetActive(true);
            currentPage = 3;
        }
        else if (currentPage == 3)
        {
            craftUI3.SetActive(false);
            craftUI1.SetActive(true);
            currentPage = 1;
        }
    }

    public void OnArmorTabClick()
    {
        PlayClickSound();

        craftUI1.SetActive(false);
        craftUI2.SetActive(false);
        craftUI3.SetActive(false);
        craftUI_armor.SetActive(true);
    }

    public void OnCraftTabClick()
    {
        PlayClickSound();

        craftUI_armor.SetActive(false);
        craftUI1.SetActive(true);
        craftUI2.SetActive(false);
        craftUI3.SetActive(false);
        currentPage = 1;
    }


    public void OnArmorSlotClick(int slotIndex)
    {
        PlayClickSound();

        selectedSlotIndex = slotIndex;
        isPreviewActivated = false;
        isActivated = false;
        go_BaseUI.SetActive(false);
        GameManager.isCraftManualOpen = false;

        GameManager.UpdateCursorState();

        BuildArmor();
    }

    private void BuildArmor()
    {
        Craft craft = craft_armor[selectedSlotIndex];

        if (!CheckMaterials_Armor(selectedSlotIndex))
        {
            Debug.Log("재료가 부족하여 장착할 수 없습니다.");
            return;
        }

        ConsumeMaterials_Armor(selectedSlotIndex);

        switch (selectedSlotIndex)
        {
            case 0:
                if (!isHelmetEquipped)
                {
                    isHelmetEquipped = true;
                    SetHelmetEquipped(true); // 상태 갱신
                    helmetIcon.SetActive(true);
                }
                break;
            case 1:
                if (!isArmorEquipped)
                {
                    isArmorEquipped = true;
                    SetArmorEquipped(true);
                    armorIcon.SetActive(true);
                }
                break;
            case 2:
                if (!isPantsEquipped)
                {
                    isPantsEquipped = true;
                    SetPantsEquipped(true);
                    pantsIcon.SetActive(true);
                }
                break;
            case 3:
                if (!isBootsEquipped)
                {
                    isBootsEquipped = true;
                    SetBootsEquipped(true);
                    bootsIcon.SetActive(true);
                }
                break;
        }

        Debug.Log($"장비 {craft.craftName} 장착 완료!");
    }
    private bool CheckMaterials_Armor(int slotIndex)
    {
        Craft craft = craft_armor[slotIndex];

        for (int i = 0; i < craft.craftNeedItem.Length; i++)
        {
            if (!inventory.HasItem(craft.craftNeedItem[i], craft.craftNeedItemCount[i]))
                return false;
        }

        return true;
    }

    private void ConsumeMaterials_Armor(int slotIndex)
    {
        Craft craft = craft_armor[slotIndex];

        for (int i = 0; i < craft.craftNeedItem.Length; i++)
        {
            inventory.ConsumeItem(craft.craftNeedItem[i], craft.craftNeedItemCount[i]);
        }
    }

    public bool IsWearingFullArmor()
    {
        return isHelmetEquipped && isArmorEquipped && isPantsEquipped && isBootsEquipped;
    }

    public bool IsWearingAnyArmor()
    {
        return isHelmetEquipped || isArmorEquipped || isPantsEquipped || isBootsEquipped;
    }
    //추위에 따른 아머 함수
    public float CalculateColdDamageAfterProtection(float rawDamage)
    {
        float totalProtection = 0f;

        if (isHelmetEquipped)
            totalProtection += helmetColdProtection;
        if (isArmorEquipped)
            totalProtection += armorColdProtection;
        if (isPantsEquipped)
            totalProtection += pantsColdProtection;
        if (isBootsEquipped)
            totalProtection += bootsColdProtection;

        totalProtection = Mathf.Clamp01(totalProtection);

        return rawDamage * (1f - totalProtection);
    }

    //얘는 재료형 함수
    private void BuildMaterial()
    {
        Craft craft = craft_fire[selectedSlotIndex];
        int amount = craft.craftAmount;

        // 재료 충분한지 확인 (제작 1회 기준)
        for (int i = 0; i < craft.craftNeedItem.Length; i++)
        {
            string itemName = craft.craftNeedItem[i];
            int needAmount = craft.craftNeedItemCount[i]; // ❗ 그대로

            if (!inventory.HasItem(itemName, needAmount))
            {
                Debug.Log("재료가 부족하여 제작할 수 없습니다!");
                return;
            }
        }

        // 재료 소모 (제작 1회 기준)
        for (int i = 0; i < craft.craftNeedItem.Length; i++)
        {
            inventory.ConsumeItem(craft.craftNeedItem[i], craft.craftNeedItemCount[i]);
        }

        // 아이템 획득: 수량만큼 생성
        inventory.AcquireItemByName(craft.itemID, amount);

        Debug.Log($"'{craft.itemID}'을(를) {amount}개 제작 완료!");
    }

    //소리 
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
    //build소리
    private void PlayBuildSound()
    {
        if (audioSource != null && buildSound != null)
            audioSource.PlayOneShot(buildSound);
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && OpenSound != null)
            audioSource.PlayOneShot(OpenSound);
    }
    //꺼지기 버튼
    public void OnClickCloseButton()
    {
        if (isPreviewActivated)
        {
            Cancel(); // 미리보기 모드면 취소 처리
        }
        CloseWindow(); // 창 닫기 처리
    }
}



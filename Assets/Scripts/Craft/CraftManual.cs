using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
    public class Craft
    {
        public string craftName; // 이름 
        public GameObject go_Prefab; // 실제 설치될 프리팹.
        public string[] craftNeedItem; //필요한 아이템
        public int[] craftNeedItemCount; //필요한 아이템 개수
        public GameObject go_PreviewPrefab; // 미리보기 프리팹.


    }

    public class CraftManual : MonoBehaviour
    {
        private Inventory inventory;

        //상태변수
        private bool isActivated = false;
        private bool isPreviewActivated = false;

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


    //crafttab ui 칸
    [SerializeField] private GameObject craftUI1;
    [SerializeField] private GameObject craftUI2;

    private int currentPage = 1;

    public void SlotClick(int _slotNumber)
    {
        if (!CheckMaterials(_slotNumber))
        {
            Debug.Log("자재가 부족하여 제작할 수 없습니다!");
            return;
        }

        selectedSlotIndex = _slotNumber;
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_Prefab;
        isPreviewActivated = true;
        targetRotation = go_Preview.transform.rotation;


        go_BaseUI.SetActive(false);

        // 마우스 커서 숨기고 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
        {
            inventory = FindObjectOfType<Inventory>(); //인벤토리 자동 검색
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

        if (Input.GetKeyDown(KeyCode.Escape)) // Esc 키로도 취소
            Cancel();
    }


    private void Build()
        {
            if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
            {
                int selectedSlot = GetSelectedSlot();  // 현재 선택된 슬롯을 가져옴 (추가 필요)

                if (!CheckMaterials(selectedSlot))
                {
                    Debug.Log("재료가 부족하여 제작할 수 없습니다!");
                    return;
                }

                ConsumeMaterials(selectedSlot);  //재료 소모
                Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
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

        private void Window()
        {
            if (!isActivated)
                OpenWindow();
            else
                CloseWindow();
        }

        private void OpenWindow()
        {

            isActivated = true;
            go_BaseUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 커서 사라짐 방지
    }

        private void CloseWindow()
        {

            isActivated = false;
            go_BaseUI.SetActive(false);

        //  커서 숨기고 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        if (currentPage == 1)
        {
            craftUI1.SetActive(false);
            craftUI2.SetActive(true);
            currentPage = 2;
        }
        else if (currentPage == 2)
        {
            craftUI2.SetActive(false);
            craftUI1.SetActive(true);
            currentPage = 1;
        }
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //진품
    public static bool hasKey = false;
    public static bool isChestUIOpen = false;
    public static bool isOpenInventory = false;
    public static bool isPauseMenuOpen = false;
    public static bool isCraftManualOpen = false;
    public static bool isSmeltingUIOpen = false;
    public static bool canPlayerMove = true;
    public static bool canPlayerRotate = true;
    public static bool escHandledThisFrame = false;
    public GameObject loadingUI;
    public static bool isLoading = false;
    public static bool isAnvilUIOpen = false;
    public static bool isShipRepairUIOpen = false;
    public static bool isTutorialOpen = false;
    public static bool isIntroPlaying = false;

    void Start()
    {
        // UI 초기화 상태 전부 false로 시작한다고 가정
        isChestUIOpen = false;
        isOpenInventory = false;
        isPauseMenuOpen = false;
        isCraftManualOpen = false;
        isSmeltingUIOpen = false;
        isAnvilUIOpen = false;
        isShipRepairUIOpen = false;

        UpdateCursorState(); // 커서 상태 강제 초기화
        StartCoroutine(HandleLoading());
    }
    void LateUpdate()
    {
        escHandledThisFrame = false;
        UpdateCursorState();
    }

    /// 커서 상태를 현재 UI 상태에 맞춰 자동 갱신
    public static void UpdateCursorState()
    {
        if (isLoading)
        {
            // 로딩 중엔 강제로 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = false;
            canPlayerRotate = false;
            return;
        }

        bool shouldShowCursor =
            isChestUIOpen || isOpenInventory ||
            isPauseMenuOpen || isCraftManualOpen || isSmeltingUIOpen ||
            isAnvilUIOpen || isShipRepairUIOpen || isTutorialOpen;

        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldShowCursor;

        canPlayerMove = !shouldShowCursor;
        canPlayerRotate = !shouldShowCursor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
            if (timeSystem != null)
                timeSystem.ToggleDayNight();
        }
    }


    public IEnumerator HandleLoading()
    {
        isLoading = true;  // 로딩 시작

        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
            CanvasGroup cg = loadingUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;

            yield return new WaitForSeconds(3f);

            float duration = 2f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (cg != null) cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
                yield return null;
            }

            loadingUI.SetActive(false);
        }

        isLoading = false;  // 로딩 끝
    }

}




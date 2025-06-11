using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    void Start()
    {
        // UI 초기화 상태 전부 false로 시작한다고 가정
        isChestUIOpen = false;
        isOpenInventory = false;
        isPauseMenuOpen = false;
        isCraftManualOpen = false;
        isSmeltingUIOpen = false;
        Debug.Log($"[Cursor Debug] Cursor.visible = {Cursor.visible}, lockState = {Cursor.lockState}");
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
        bool shouldShowCursor =
            isChestUIOpen || isOpenInventory ||
            isPauseMenuOpen || isCraftManualOpen|| isSmeltingUIOpen;

        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldShowCursor;

        canPlayerMove = !shouldShowCursor;
        canPlayerRotate = !shouldShowCursor;
    }

    public IEnumerator HandleLoading()
    {
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
    }

}

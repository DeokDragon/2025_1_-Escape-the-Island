using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool hasKey = false;
    public static bool isChestUIOpen = false;
    public static bool canPlayerMove = true;
    public static bool isOpenInventory = false;
    public static bool canPlayerRotate = true;
    public static bool escHandledThisFrame = false;

    public GameObject loadingUI; // 로딩 화면 UI 연결

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetCursorState(false);

        StartCoroutine(HandleLoading()); // 로딩 코루틴 실행
    }

    // Update is called once per frame
    void Update()
    {
        // 여기에 다른 입력 처리 가능
    }

    void LateUpdate()
    {
        GameManager.escHandledThisFrame = false; // 모든 Update 이후에 초기화
        Debug.Log("🔄 escHandledThisFrame 리셋됨");
    }

    // 커서 설정
    private void SetCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }
    }

    // 로딩 UI 표시 코루틴
    IEnumerator HandleLoading()
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);

            CanvasGroup cg = loadingUI.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = 1f;

            yield return new WaitForSeconds(3f); // 로딩 표시 시간

            // ⬇ 페이드 아웃
            float duration = 2f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (cg != null)
                    cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
                yield return null;
            }

            loadingUI.SetActive(false);
        }
    }

}

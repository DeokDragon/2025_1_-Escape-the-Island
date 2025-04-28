using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true;
    public static bool isOpenInventory = false;

    // Start is called before the first frame update
    void Start()
    {
        // 게임 시작 시 커서 잠금 및 숨김 처리
        SetCursorState(false);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    // 커서 상태 설정 함수 (true일 경우 커서 보임, false일 경우 숨김)
    private void SetCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;  // 커서 잠금 해제
            Cursor.visible = true;  // 커서 보이게 설정
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;  // 커서 잠금
            Cursor.visible = false;  // 커서 숨김
        }
    }
}

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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        SetCursorState(false);
    }

    // Update is called once per frame
    void Update()
    {

       

    }

    void LateUpdate()
    {
        escHandledThisFrame = false; // 모든 Update 이후에 초기화
    }
    // 커서 설정
    private void SetCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;  // 커서 설정
            Cursor.visible = true;  // 커서 보이기
            canPlayerMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }
    }
}

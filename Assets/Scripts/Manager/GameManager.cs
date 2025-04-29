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
        // ���� ���� �� Ŀ�� ��� �� ���� ó��
        SetCursorState(false);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    // Ŀ�� ���� ���� �Լ� (true�� ��� Ŀ�� ����, false�� ��� ����)
    private void SetCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;  // Ŀ�� ��� ����
            Cursor.visible = true;  // Ŀ�� ���̰� ����
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;  // Ŀ�� ���
            Cursor.visible = false;  // Ŀ�� ����
        }
    }
}

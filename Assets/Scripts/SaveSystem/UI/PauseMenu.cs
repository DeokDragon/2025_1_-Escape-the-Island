using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public int saveSlotIndex = 0; // 저장할 슬롯 번호. 일단 기본 0번 슬롯

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.isChestUIOpen && !GameManager.escHandledThisFrame)
            {
                TogglePause();
                GameManager.escHandledThisFrame = true; // 혹시 몰라 여기서도 설정해주면 더 안전
            }
        }
    }

    public void TogglePause()
    {
        bool isActive = pauseUI.activeSelf;
        pauseUI.SetActive(!isActive);

        // 마우스 커서 고정 해제
        Cursor.visible = !isActive;
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;

        // 플레이어 움직임 잠금 (선택사항)
        GameManager.canPlayerMove = isActive;
    }

    public void OnClickSave()
    {
       

        if (SaveManager.instance == null)
        {
            Debug.LogError("❌ SaveManager.instance == null !!! 저장 불가능");
        }
        else
        {
            SaveManager.instance.SaveToSlot(saveSlotIndex);
        }
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    

}

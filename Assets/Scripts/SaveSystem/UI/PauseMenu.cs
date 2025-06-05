using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
   

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
        int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", 0); //  현재 선택된 슬롯 불러오기

        if (SaveManager.instance == null)
        {
            Debug.LogError("❌ SaveManager.instance == null !!! 저장 불가능");
        }
        else
        {
            Debug.Log($" 저장 시도: 슬롯 {selectedSlot}");
            SaveManager.instance.SaveToSlot(selectedSlot); //  선택된 슬롯에 저장
        }
    }


    public void OnClickExit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickSettings()
    {
        PlayerPrefs.SetString("SettingsCaller", "Game");
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }



}

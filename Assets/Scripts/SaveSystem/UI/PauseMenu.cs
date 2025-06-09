using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject PauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. 설정창이 열려 있는 경우 → 설정창만 닫기
            Scene settingsScene = SceneManager.GetSceneByName("SettingScene");
            if (settingsScene.IsValid() && settingsScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(settingsScene);

                GameObject pauseMenu = GameObject.Find("PauseMenuUI");
                if (pauseMenu != null)
                    pauseMenu.SetActive(true);

                Debug.Log("🔙 ESC로 설정창 닫음");
                return; // ⛔ 여기서 바로 return해서 아래 Pause 토글 방지
            }

            // 2. 일반 ESC 동작 (기존 로직)
            if (!GameManager.isChestUIOpen && !GameManager.escHandledThisFrame)
            {
                TogglePause();
                GameManager.escHandledThisFrame = true;
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
        SceneManager.LoadScene("SettingScene", LoadSceneMode.Additive);

        if (PauseMenuUI != null)
            PauseMenuUI.SetActive(false); // 👈 PauseMenu 끄기
    }





}
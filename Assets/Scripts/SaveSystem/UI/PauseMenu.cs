using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject PauseMenuUI;

    void Update()
    {
        
        
            Debug.Log($"[DEBUG] PauseMenu: Cursor.visible = {Cursor.visible}, lockState = {Cursor.lockState}");

            
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. 설정창 켜져 있으면 → 설정창 닫기
            Scene settingsScene = SceneManager.GetSceneByName("SettingScene");
            if (settingsScene.IsValid() && settingsScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(settingsScene);
                GameObject pauseMenu = GameObject.Find("PauseMenuUI");
                if (pauseMenu != null)
                    pauseMenu.SetActive(true);
                Debug.Log("🔙 ESC로 설정창 닫음");
                return;
            }

            // 2. PauseMenu 토글
            TogglePause();
        }
    }




    public void TogglePause()
    {
        bool isActive = pauseUI.activeSelf;
        pauseUI.SetActive(!isActive);

        if (!isActive)
        {
            // PauseMenu 켜질 때
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // PauseMenu 꺼질 때
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        GameManager.canPlayerMove = !pauseUI.activeSelf;
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
        int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", -1);

        if (selectedSlot != -1 && SaveManager.instance != null)
        {
            SaveManager.instance.SaveToSlot(selectedSlot);
            Debug.Log($"💾 게임 종료 직전 자동 저장 완료! 슬롯: {selectedSlot}");
        }
        else
        {
            Debug.LogWarning("❌ 자동 저장 실패 – 선택된 슬롯이 없거나 SaveManager가 없음");
        }

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
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject PauseMenuUI;
    //참조
    private CraftManual craftManual;



    void Start()
    {
        craftManual = FindObjectOfType<CraftManual>();
    }
    void Update()
    {
        if (GameManager.escHandledThisFrame)
            return;

        if (GameManager.isChestUIOpen)
            return;

        // 1. 제작 UI(CraftManual)가 열려있을 경우 → ESC로 취소 처리
        if (craftManual != null && craftManual.IsUIActivated())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                craftManual.CancelCraft();
                GameManager.escHandledThisFrame = true;
            }
            return;
        }

        // 2. ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 2-1. 설정창이 열려 있다면 → 설정창 닫기
            Scene settingsScene = SceneManager.GetSceneByName("SettingScene");
            if (settingsScene.IsValid() && settingsScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(settingsScene);

                GameObject pauseMenu = GameObject.Find("PauseMenuUI");
                if (pauseMenu != null)
                    pauseMenu.SetActive(true);

                Debug.Log("🔙 ESC로 설정창 닫음");
                GameManager.escHandledThisFrame = true;
                return;
            }

            // 2-2. 설정창 안 열려 있으면 → Pause 메뉴 토글
            TogglePause();
            GameManager.escHandledThisFrame = true;
        }
    }

    void LateUpdate()
    {
        GameManager.escHandledThisFrame = false;
    }


    public void TogglePause()
    {
        bool isActive = pauseUI.activeSelf;
        pauseUI.SetActive(!isActive);

        GameManager.isPauseMenuOpen = !isActive;
        GameManager.UpdateCursorState(); // 커서 자동 처리
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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public Button[] keyButtons; // 순서대로: W, A, S, D, E, I
    public TextMeshProUGUI[] keyTexts;

    private string[] keyActions = { "MoveUp", "MoveLeft", "MoveDown", "MoveRight", "Interact", "Inventory" };
    private int waitingForKeyIndex = -1;

    void Start()
    {


        if (sfxSlider == null)
            Debug.LogError("❌ sfxSlider가 연결되지 않았습니다!");
        else
            Debug.Log("✅ sfxSlider 정상 연결됨");

        // Load saved sound settings
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bgmSlider.onValueChanged.AddListener((v) => SoundManager.instance.SetBGMVolume(v));
        sfxSlider.onValueChanged.AddListener((v) => SoundManager.instance.SetSFXVolume(v));

        for (int i = 0; i < keyButtons.Length; i++)
        {
            int index = i;
            keyButtons[i].onClick.AddListener(() => WaitForKey(index));
            keyTexts[i].text = PlayerPrefs.GetString(keyActions[i], GetDefaultKey(keyActions[i])).ToUpper();
        }
    }

    void Update()
    {
        if (waitingForKeyIndex >= 0)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    string action = keyActions[waitingForKeyIndex];
                    PlayerPrefs.SetString(action, key.ToString());
                    keyTexts[waitingForKeyIndex].text = key.ToString().ToUpper();
                    waitingForKeyIndex = -1;
                    break;
                }
            }
        }
    }

    void WaitForKey(int index)
    {
        waitingForKeyIndex = index;
        keyTexts[index].text = "Press Key...";
    }

    string GetDefaultKey(string action)
    {
        switch (action)
        {
            case "MoveUp": return "W";
            case "MoveLeft": return "A";
            case "MoveDown": return "S";
            case "MoveRight": return "D";
            case "Interact": return "E";
            case "Inventory": return "I";
            default: return "";
        }
    }

    public void OnClickClose()
    {
        string caller = PlayerPrefs.GetString("SettingsCaller", "Game");

        if (caller == "Game")
        {
            Scene settingsScene = SceneManager.GetSceneByName("SettingsScene");
            if (settingsScene.IsValid() && settingsScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(settingsScene);
            }

            GameObject pauseMenu = GameObject.Find("PauseMenuUI");
            if (pauseMenu != null)
                pauseMenu.SetActive(true);
            else
                Debug.LogWarning("❌ PauseMenuUI 못 찾음");
        }
        else if (caller == "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }


    public void OnClickApply()
    {
        Debug.Log(" 저장된 키값 즉시 적용 완료");

        // 예시: 리셋하고 다시 불러오기
        for (int i = 0; i < keyActions.Length; i++)
        {
            string key = PlayerPrefs.GetString(keyActions[i], GetDefaultKey(keyActions[i]));
            keyTexts[i].text = key.ToUpper(); // UI 갱신도 보장
        }

        // 여기에 필요하면 추가로 사운드도 적용 가능
        SoundManager.instance.SetBGMVolume(bgmSlider.value);
        SoundManager.instance.SetSFXVolume(sfxSlider.value);
    }

}

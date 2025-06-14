﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("SaveSlot");  //  세이브 슬롯 화면으로 연결
        
    }

    public void OnClickSettings()
    {
        PlayerPrefs.SetString("SettingsCaller", "MainMenu");
        SceneManager.LoadScene("SettingScene");
    }


    public void OnClickQuit()
    {
        Debug.Log("게임 종료됨");

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; // 에디터 테스트용 종료
#else
        Application.Quit(); // 빌드했을 때 종료
#endif
    }


    public void OnClickSlot1Continue()
    {
        

        PlayerPrefs.SetInt("IsContinue", 1); // 이어하기
        PlayerPrefs.SetInt("SelectedSlot", 0); // slot0
        SceneManager.LoadScene("Game"); // Game 씬으로 이동

    }

    public void OnClickSlot1NewGame()
    {
        PlayerPrefs.SetInt("IsContinue", 0); // 새 게임
        PlayerPrefs.SetInt("SelectedSlot", 0); // slot0
        SceneManager.LoadScene("Game");
    }

  
}

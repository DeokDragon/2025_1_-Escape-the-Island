using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    public void OnClickSlot(int slotIndex)
    {
        PlayerPrefs.SetInt("SelectedSlot", slotIndex);

        if (SaveManager.instance.HasSaveFile(slotIndex))
        {
            Debug.Log($"슬롯 {slotIndex + 1}: 저장 파일 존재 → 이어하기");

            PlayerPrefs.SetInt("IsContinue", 1); // ← 이거 빠졌음!!
        }
        else
        {
            Debug.Log($"슬롯 {slotIndex + 1}: 저장 없음 → 새 게임 시작");
            PlayerPrefs.SetInt("IsContinue", 0);
        }


        SceneManager.LoadScene("Game"); 
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

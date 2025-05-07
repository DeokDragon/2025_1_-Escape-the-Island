using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    public void OnClickSlot(int slotIndex)
    {
        PlayerPrefs.SetInt("SelectedSlot", slotIndex);

        if (SaveManager.instance.HasSaveFile(slotIndex))
        {
            

            PlayerPrefs.SetInt("IsContinue", 1); // ← 이거 빠졌음!!
        }
        else
        {
           
            PlayerPrefs.SetInt("IsContinue", 0);
        }


        SceneManager.LoadScene("Game"); 
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

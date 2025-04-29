using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    public void OnClickSlot(int slotIndex)
    {
        PlayerPrefs.SetInt("SelectedSlot", slotIndex);

        if (SaveManager.instance.HasSaveFile(slotIndex))
        {
            Debug.Log($"���� {slotIndex + 1}: ���� ���� ���� �� �̾��ϱ�");

            PlayerPrefs.SetInt("IsContinue", 1); // �� �̰� ������!!
        }
        else
        {
            Debug.Log($"���� {slotIndex + 1}: ���� ���� �� �� ���� ����");
            PlayerPrefs.SetInt("IsContinue", 0);
        }


        SceneManager.LoadScene("Game"); 
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

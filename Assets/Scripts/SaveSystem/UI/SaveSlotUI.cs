using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // 🔥 반드시 추가

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private Button[] slotButtons;

    void Start()
    {
        if (slotButtons == null)
        {
            Debug.LogError("❌ slotButtons 배열이 null입니다!");
            return;
        }

        Debug.Log($"✅ slotButtons 길이: {slotButtons.Length}");

        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] == null)
                Debug.LogError($"❌ slotButtons[{i}] 가 null입니다");
            else
                Debug.Log($"✅ slotButtons[{i}] 연결 완료: {slotButtons[i].name}");
        }

        if (SaveManager.instance == null)
        {
            Debug.LogError("❌ SaveManager.instance 가 null입니다!");
            return;
        }

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnClickSlot(index));

            // ✅ TMP 대응 버전!
            TextMeshProUGUI textComponent = slotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                if (!SaveManager.instance.HasSaveFile(index))
                    textComponent.text = $"파일 {index + 1} (새로 시작)";
                else
                    textComponent.text = $"파일 {index + 1} (이어하기)";
            }
            else
            {
                Debug.LogWarning($"⚠️ Btn_Slot{index + 1}에 TMP_Text가 없음!");
            }
        }
    }

    public void OnClickSlot(int slotIndex)
    {
        PlayerPrefs.SetInt("SelectedSlot", slotIndex);
        PlayerPrefs.SetInt("IsContinue", SaveManager.instance.HasSaveFile(slotIndex) ? 1 : 0);
        SceneManager.LoadScene("Game");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroStoryUI : MonoBehaviour
{
    [System.Serializable]
    public class StoryPage
    {
        public string imagePath; // Resources ���
        [TextArea] public string storyText;
    }

    public StoryPage[] storyPages;

    public Image storyImage;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI hintText;
    public GameObject storyPanel;

    private int currentPage = 0;

    void Start()
    {
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);

        if (isContinue == 1)
        {
            // �̾��ϱ��� ���� ���丮 �г� �ȶ��
            storyPanel.SetActive(false);
            GameManager.canPlayerMove = true;
            return;
        }

        currentPage = 0;
        storyPanel.SetActive(true);
        GameManager.canPlayerMove = false;
        ShowPage();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextPage();
        }
    }

    void ShowPage()
    {
        if (currentPage < storyPages.Length)
        {
            var page = storyPages[currentPage];
            storyText.text = page.storyText;

            Sprite img = Resources.Load<Sprite>(page.imagePath);
            if (img != null)
                storyImage.sprite = img;
        }
        else
        {
            EndStory();
        }
    }

    void NextPage()
    {
        currentPage++;
        ShowPage();
    }

    void EndStory()
    {
        storyPanel.SetActive(false);
        GameManager.canPlayerMove = true;  // �� ���� ����
    }
}

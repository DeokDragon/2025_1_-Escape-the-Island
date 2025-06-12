using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroStoryUI : MonoBehaviour
{
    [System.Serializable]
    public class StoryPage
    {
        public string imagePath; // Resources 경로
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
        currentPage = 0;
        storyPanel.SetActive(true);
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
        GameManager.canPlayerMove = true;  // 본 게임 시작
    }
}

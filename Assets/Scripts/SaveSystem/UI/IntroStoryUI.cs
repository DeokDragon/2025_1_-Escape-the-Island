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
    public GameObject player;  // 플레이어 오브젝트 연결

    private int currentPage = 0;

    void Start()
    {
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);

        if (isContinue == 1)
        {
            storyPanel.SetActive(false);
            GameManager.canPlayerMove = true;
            GameManager.canPlayerRotate = true;
            GameManager.isIntroPlaying = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        currentPage = 0;
        storyPanel.SetActive(true);
        GameManager.canPlayerMove = false;
        GameManager.canPlayerRotate = false;
        GameManager.isIntroPlaying = true;

        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
            else
                Debug.LogWarning("스토리 이미지 로드 실패: " + page.imagePath);
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

        GameManager.canPlayerMove = true;
        GameManager.canPlayerRotate = true;
        GameManager.isIntroPlaying = false;

        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = true;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

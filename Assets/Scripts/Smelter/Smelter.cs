using UnityEngine;

public class Smelter : MonoBehaviour
{
    public GameObject interactPromptUI;     // "E" 프롬프트        
    public Transform player;                // 플레이어 Transform
    public float interactDistance = 3f;

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    private readonly string[] smeltableItems = { "Iron" };  //이걸 iron stone으로...?

    [SerializeField] private GameObject smeltingUI; // 제련 UI 창 (Canvas 자식으로 만들어두고 꺼두기)

    void Update()
    {
        if (isUIOpen)
        {
            interactPromptUI.SetActive(false); // UI 열려있을 땐 프롬프트 꺼두기

            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseSmeltingUI();
                GameManager.escHandledThisFrame = true;
            }
            return;
        }

        CheckPlayerNear();
        TrySmeltingInput();
    }


    void CheckPlayerNear()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            isPlayerNear = true;
            interactPromptUI.SetActive(true); // 무조건 표시
        }
        else
        {
            isPlayerNear = false;
            interactPromptUI.SetActive(false);
        }
    }

    void TrySmeltingInput()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OpenSmeltingUI(); // 그냥 바로 UI 띄우기
        }
    }
    void OpenSmeltingUI()
    {
        if (smeltingUI != null)
        {
            smeltingUI.SetActive(true);
            isUIOpen = true;

            GameManager.isSmeltingUIOpen = true;
            GameManager.UpdateCursorState();
        }
    }

    public void CloseSmeltingUI()
    {
        smeltingUI.SetActive(false);
        isUIOpen = false;

        GameManager.isSmeltingUIOpen = false;
        GameManager.UpdateCursorState();

        interactPromptUI.SetActive(false);
    }
    public void TrySmelting()
    {
        Debug.Log("용광로 UI 열기");
        OpenSmeltingUI();
    }
}
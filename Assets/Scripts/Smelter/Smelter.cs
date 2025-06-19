using UnityEngine;

public class Smelter : MonoBehaviour
{
    public GameObject interactPromptUI;     // "E" ������Ʈ        
    public Transform player;                // �÷��̾� Transform
    public float interactDistance = 3f;

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    private readonly string[] smeltableItems = { "Iron" };  //�̰� iron stone����...?

    [SerializeField] private GameObject smeltingUI; // ���� UI â (Canvas �ڽ����� �����ΰ� ���α�)

    void Update()
    {
        if (isUIOpen)
        {
            interactPromptUI.SetActive(false); // UI �������� �� ������Ʈ ���α�

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
            interactPromptUI.SetActive(true); // ������ ǥ��
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
            OpenSmeltingUI(); // �׳� �ٷ� UI ����
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
        Debug.Log("�뱤�� UI ����");
        OpenSmeltingUI();
    }
}
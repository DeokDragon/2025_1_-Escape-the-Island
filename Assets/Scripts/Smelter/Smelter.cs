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

            if (Input.GetKeyDown(KeyCode.Escape))
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

            

            GameManager.canPlayerMove = false;
            GameManager.canPlayerRotate = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseSmeltingUI()
    {
        smeltingUI.SetActive(false);
        isUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        interactPromptUI.SetActive(false);

        GameManager.canPlayerMove = true;
        GameManager.canPlayerRotate = true;
    }
    public void TrySmelting()
    {
        Debug.Log("�뱤�� UI ����");
        OpenSmeltingUI();
    }
}
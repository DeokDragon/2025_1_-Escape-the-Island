using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WreckShipRepair : MonoBehaviour
{

    public GameObject[] shipModelStages;
    public int woodRequired = 40;
    public string requiredItemName = "Log"; // "Log" ������
    private int currentWood = 0;

    private bool isPlayerInRange = false;
    private bool canRepair = true;

    private float lastRepairTime = 0f;
    private float repairCooldown = 0.2f;

    // �θ� ��ü�� Transform�� �̿��Ͽ� ��ġ�� ȸ���� ����
    private Vector3 basePosition;
    private Quaternion baseRotation;

    public AudioClip repairSound; // ���� ����
    private AudioSource audioSource; // AudioSource ������Ʈ

    void Start()
    {
        // �θ� ��ü�� �⺻ ��ġ�� ȸ������ ����
        basePosition = transform.position;
        baseRotation = transform.rotation;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && canRepair)
        {
            StartCoroutine(RepairShipCoroutine());
        }
    }

    private IEnumerator RepairShipCoroutine()
    {
        canRepair = false;

        if (currentWood >= woodRequired)
        {
            Debug.Log("���ļ� �̹� ���� �Ϸ�");
            canRepair = true;
            yield break;
        }

        // �κ��丮���� ������ ����� �ִ��� üũ
        if (Inventory.instance.HasItem(requiredItemName, 1))
        {
            Inventory.instance.ConsumeItem(requiredItemName, 1);
            currentWood++;
            UpdateShipModel();
            Debug.Log("���� �߰���! ���� ����: " + currentWood);

            // ���� ���
            if (repairSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(repairSound);
            }
        }
        else
        {
            Debug.Log("���� ����! ������ ������� �ʾҽ��ϴ�.");
        }

        yield return new WaitForSeconds(0.3f); // 0.3�� ������ �߰�
        canRepair = true;
    }

    private void UpdateShipModel()
    {
        int stageCount = shipModelStages.Length - 1; // ���������� �� ����
        int stage = Mathf.Clamp(currentWood * stageCount / woodRequired, 0, stageCount); // ���� ���� ���� �������� ���

        Debug.Log($"[UpdateShipModel] ���� ����: {currentWood}, ���� ��������: {stage}");

        for (int i = 0; i < shipModelStages.Length; i++)
        {
            bool isActive = i == stage; // �ش� ���������� �𵨸� Ȱ��ȭ
            shipModelStages[i].SetActive(isActive); // Ȱ��ȭ/��Ȱ��ȭ ó��

            Debug.Log($"�� {shipModelStages[i].name} {(isActive ? "Ȱ��ȭ" : "��Ȱ��ȭ")}");

            // Ȱ��ȭ�� ���� ��ġ�� ȸ������ ���ؿ� ���߱�
            if (isActive)
            {
                Vector3 basePosition = shipModelStages[0].transform.position;
                Quaternion baseRotation = shipModelStages[0].transform.rotation;
                shipModelStages[i].transform.position = basePosition;
                shipModelStages[i].transform.rotation = baseRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("�÷��̾� ���ļ� ���� ����");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("�÷��̾� ���ļ� ���� ��Ż");
        }
    }
}
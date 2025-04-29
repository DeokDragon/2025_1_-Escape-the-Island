using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : MonoBehaviour
{
   
    [SerializeField] private float fireRadius = 5f;  // ���� ������ ��ġ�� �ݰ�
    [SerializeField] private float temperatureIncreaseRate = 0.3f;  // �� ��ó���� ü�� ���� ����

    private DayAndNight dayAndNightScript;  // DayAndNight ��ũ��Ʈ ����

    void Start()
    {
        // DayAndNight ��ũ��Ʈ ã��
        dayAndNightScript = FindObjectOfType<DayAndNight>();
    }

    void Update()
    {
        // Fire ��ü�� �÷��̾��� �Ÿ� ���
        Collider[] colliders = Physics.OverlapSphere(transform.position, fireRadius);

        foreach (Collider collider in colliders)
        {
            // �÷��̾ ���� ���� ������ ������ ��
            if (collider.CompareTag("Player"))
            {
                // �÷��̾���� �Ÿ� ���
                float distanceToFire = Vector3.Distance(collider.transform.position, transform.position);

                // �Ұ��� �Ÿ� ��ʷ� �µ� ������ ���
                float temperatureIncrease = Mathf.Lerp(0, temperatureIncreaseRate, 1 - (distanceToFire / fireRadius));

                // �µ� ����
                if (dayAndNightScript != null)
                {
                    dayAndNightScript.SetTemperature(dayAndNightScript.GetTemperature() + temperatureIncrease);
                }
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond;
    [SerializeField] private Text temperatureText; // ȭ�鿡 ǥ�õ� �µ� �ؽ�Ʈ

    private bool isNight = false;

    [SerializeField] private float fogDensityCalc;
    [SerializeField] private float defaultDayFogDensity = 0.01f;
    [SerializeField] private float nightFogDensity = 0.1f;  // ���� �Ȱ� �е�
    private float dayFogDensity;
    private float currentFogDensity;
    private float targetFogDensity;

    // �µ� ���� ����
    [SerializeField] private float temperature = 10f;  // ���� �µ�
    [SerializeField] private float coldDamageRate = 1f; // ������ ���� ü�� ������
    [SerializeField] private float minTemperature = -10f;  // ���� �µ�
    [SerializeField] private float maxTemperature = 25f;  // �ְ� �µ�
    [SerializeField] private float temperatureChangeSpeed = 1f; // �µ� ��ȭ �ӵ�

    [SerializeField] private GameObject fireObject;  // ��ں� ������Ʈ
    [SerializeField] private float fireRadius = 5f;  // ��ں��� ������ ��ġ�� �ݰ�

    private StatusController statusController;  // �÷��̾� ���� ����
    private Transform playerTransform;  // �÷��̾� Transform

    // Start is called before the first frame update
    void Start()
    {
        statusController = FindObjectOfType<StatusController>();

        // �÷��̾� ��ü�� ã�Ƽ� Transform�� �����ɴϴ�.
        playerTransform = GameObject.FindWithTag("Player").transform;  // �÷��̾ ã��

        dayFogDensity = defaultDayFogDensity;

        // �ʱ� ����
        currentFogDensity = dayFogDensity;
        targetFogDensity = dayFogDensity;
        RenderSettings.fogDensity = currentFogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        float angleX = transform.eulerAngles.x;
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        // ��/�� ����
        if (angleX >= 170f && angleX < 340f)
            isNight = true;
        else
            isNight = false;

        // �µ� ��ȭ ó��
        UpdateTemperature();

        if (temperatureText != null)
        {
            temperatureText.text = $"{temperature:F1}��C";
        }

        // �Ȱ� �е� ����
        if (!isNight) // ��
        {
            targetFogDensity = dayFogDensity;
        }
        else // ��
        {
            targetFogDensity = nightFogDensity;
        }

        // �Ȱ� �е� ��ȭ
        currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, fogDensityCalc * Time.deltaTime);
        RenderSettings.fogDensity = currentFogDensity;

        // ü���� ������ ü�� ����
        if (temperature < 0)
        {
            // ü���� ���� ��� ü�� ����
            float damageAmount = coldDamageRate * Time.deltaTime;
            statusController.DecreaseHP((int)damageAmount);
        }

        // ��ں� ��ó���� �µ� ��� ó��
        if (fireObject != null)
        {
            AdjustTemperatureNearFire();
        }
    }

    private void AdjustTemperatureNearFire()
    {
        // �÷��̾�� ��ں� ������ �Ÿ� ���
        float distanceToFire = Vector3.Distance(playerTransform.position, fireObject.transform.position); // �÷��̾��� ��ġ�� ���

        // ��ں� ��ó�ϼ��� �µ��� ����ϵ��� ó��
        if (distanceToFire <= fireRadius)
        {
            // �Ÿ��� �������� �µ��� ������ ���
            float temperatureIncrease = Mathf.Lerp(0, 5, 1 - (distanceToFire / fireRadius));
            temperature += temperatureIncrease * Time.deltaTime;

            // �µ� ���� (�ּ�, �ִ� �µ� ���� ����)
            temperature = Mathf.Clamp(temperature, minTemperature, maxTemperature);
        }
    }

    private void UpdateTemperature()
    {
        float angle = transform.eulerAngles.x;

        if (angle >= 170 && angle <= 340)  // �� (170�� ~ 340�� ����)
        {
            temperature = Mathf.Lerp(temperature, minTemperature, temperatureChangeSpeed * Time.deltaTime);
        }
        else  // ��
        {
            temperature = Mathf.Lerp(temperature, maxTemperature, temperatureChangeSpeed * Time.deltaTime);
        }
    }



    public void SetTemperature(float newTemperature)
    {
        temperature = Mathf.Clamp(newTemperature, minTemperature, maxTemperature); // ü���� �ּ�/�ִ� ������ ����
    }

    public float GetTemperature()
    {
        return temperature;
    }

    public void SetTime(float angleX)
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(angleX, currentRotation.y, currentRotation.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{ 
 [SerializeField] private float secondPerRealTimeSecond;
[SerializeField] private Text temperatureText; // �µ� �ؽ�Ʈ

private bool isNight = false;

[SerializeField] private float fogDensityCalc;
[SerializeField] private float defaultDayFogDensity = 0.01f;
[SerializeField] private float nightFogDensity = 0.1f;  // ���� �ִ� �Ȱ� �е�
private float dayFogDensity;
private float currentFogDensity;
private float targetFogDensity;

// �µ� ���� ����
[SerializeField] private float temperature = 10f;  // �ʱ� �µ� (����)
[SerializeField] private float coldDamageRate = 1f; // ���� �µ����� �ʴ� ü�� ���ҷ�
[SerializeField] private float minTemperature = -10f;  // �㿡 ���� �µ�
[SerializeField] private float maxTemperature = 25f;  // ���� �ְ� �µ�
[SerializeField] private float temperatureChangeSpeed = 1f; // �µ� ��ȭ �ӵ�

private StatusController statusController;  // StatusController�� ����

// Start is called before the first frame update
void Start()
{
    statusController = FindObjectOfType<StatusController>();
    dayFogDensity = defaultDayFogDensity;

    // ��ħ���� �����Ѵٰ� �����ϰ� �� �Ȱ� ������ �ʱ�ȭ
    currentFogDensity = dayFogDensity;
    targetFogDensity = dayFogDensity;
    RenderSettings.fogDensity = currentFogDensity;
}

// Update is called once per frame
void Update()
{
    float angleX = transform.eulerAngles.x;
    transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

    // ��� ���� �Ǻ�
    if (angleX >= 170f && angleX < 340f)
        isNight = true;
    else
        isNight = false;

    // �µ� ��ȭ ó��
    UpdateTemperature();

    if (temperatureText != null)
    {
        temperatureText.text = $"���� �µ�: {temperature:F1}��C";
    }

    // �Ȱ� ���� ����
    if (!isNight) // ���� ��
    {
        targetFogDensity = dayFogDensity;
    }
    else // ���� ��
    {
        targetFogDensity = nightFogDensity;
    }

    // �Ȱ� �е� ������ ��ȭ
    currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, fogDensityCalc * Time.deltaTime);
    RenderSettings.fogDensity = currentFogDensity;

    // �µ��� ���� ü�� ���� ó��
    if (temperature < 0)
    {
        // ������ �� ü�� ����
        float damageAmount = coldDamageRate * Time.deltaTime;
        statusController.DecreaseHP((int)damageAmount); // StatusController�� ü�� ���� �޼ҵ� ȣ��
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

// �ܺο��� �µ��� ������ �� �ִ� �޼ҵ�
public void SetTemperature(float newTemperature)
{
    temperature = newTemperature;
}

public float GetCurrentTime()
{
    return transform.eulerAngles.x;
}

public void SetTime(float angleX)
{
    Vector3 currentRotation = transform.eulerAngles;
    transform.eulerAngles = new Vector3(angleX, currentRotation.y, currentRotation.z);
}


}
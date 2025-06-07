using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    // -------------------- �ð� --------------------
    [Header("�ð� ����")]
    [SerializeField] private float secondPerRealTimeSecond = 60f;

    // -------------------- UI --------------------
    [Header("�µ� UI")]
    [SerializeField] private Text temperatureText;
    [SerializeField] private RectTransform thermometerFill;
    [SerializeField] private float thermometerMaxHeight = 200f;

    // -------------------- �µ� --------------------
    [Header("�µ� ����")]
    [SerializeField] private float temperature = 10f;
    [SerializeField] private float minTemperature = -10f;
    [SerializeField] private float maxTemperature = 25f;
    [SerializeField] private float temperatureChangeSpeed = 1f;
    [SerializeField] private float coldDamageRate = 1f;

    [Header("�Ȱ� ����")]
    [SerializeField] private float fogDensityCalc = 1f;
    [SerializeField] private float defaultDayFogDensity = 0.01f;
    [SerializeField] private float morningFogDensity = 0.05f;  // ��ħ �Ȱ� �е� �߰�
    [SerializeField] private float nightFogDensity = 0.1f;
    [SerializeField] private Color dayFogColor = Color.gray;
    [SerializeField] private Color morningFogColor = new Color(0.7f, 0.7f, 0.8f); // ��ħ �Ȱ��� (���� ȸ��/�Ķ���)
    [SerializeField] private Color nightFogColor = Color.black;
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    // -------------------- ���� --------------------
    [Header("���� ����")]
    [SerializeField] private Light directionalLight;
    // playerFlashlight ���� ���� ����
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private Color nightLightColor = new Color(0.2f, 0.2f, 0.4f);
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float nightLightIntensity = 0.1f;

    // -------------------- ����Ʈ ���μ��� --------------------
    [Header("����Ʈ ���μ���")]
    [SerializeField] private Volume dayVolume;
    [SerializeField] private Volume nightVolume;

    // -------------------- ���� ���� --------------------
    private bool isNight = false;
    private float dayFogDensity;
    private float currentFogDensity;
    private float targetFogDensity;
    private float hpLossBuffer = 0f;

    private StatusController statusController;
    private Transform playerTransform;

    // -------------------- Craft ����  --------------------
    private CraftManual craftManual;

    // -------------------- �ʱ� ���� --------------------
    void Start()
    {
        statusController = FindObjectOfType<StatusController>();
        craftManual = FindObjectOfType<CraftManual>();
        playerTransform = GameObject.FindWithTag("Player")?.transform;

        dayFogDensity = defaultDayFogDensity;
        currentFogDensity = targetFogDensity = dayFogDensity;

        RenderSettings.fogDensity = currentFogDensity;
        RenderSettings.fogColor = dayFogColor;

        if (dayVolume != null) dayVolume.weight = 1f;
        if (nightVolume != null) nightVolume.weight = 0f;
    }

    // -------------------- �� ������ ó�� --------------------
    void Update()
    {
        UpdateSkybox();
        RotateSun();
        UpdateDayNightState();
        UpdateTemperature();
        UpdateTemperatureUI();
        UpdateFog();
        UpdateLighting();
        UpdatePostProcessing();
        ApplyColdDamage();
    }

    // -------------------- �¾� ȸ�� --------------------
    private void RotateSun()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);
    }

    // -------------------- ��/�� �Ǻ� --------------------
    private void UpdateDayNightState()
    {
        float angleX = transform.eulerAngles.x;
        isNight = angleX >= 170f && angleX < 340f;
    }

    // -------------------- �µ� ��ȭ --------------------
    private void UpdateTemperature()
    {
        float targetTemp = isNight ? minTemperature : maxTemperature;
        temperature = Mathf.Lerp(temperature, targetTemp, temperatureChangeSpeed * Time.deltaTime);
    }

    private void UpdateTemperatureUI()
    {
        if (temperatureText != null)
            temperatureText.text = $"{temperature:F1}��C";

        if (thermometerFill != null)
        {
            float normalized = Mathf.InverseLerp(minTemperature, maxTemperature, temperature);
            float height = normalized * thermometerMaxHeight;
            thermometerFill.sizeDelta = new Vector2(thermometerFill.sizeDelta.x, height);
        }
    }

    // -------------------- �Ȱ� ���� --------------------
    private void UpdateFog()
    {
        float angleX = transform.eulerAngles.x;
        if (isNight)
        {
            targetFogDensity = nightFogDensity;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, nightFogColor, fogDensityCalc * Time.deltaTime);
        }
        else if (angleX >= 0f && angleX <= 60f)  // ��ħ ����
        {
            targetFogDensity = morningFogDensity;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, morningFogColor, fogDensityCalc * Time.deltaTime);
        }
        else  // ��
        {
            targetFogDensity = defaultDayFogDensity;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, dayFogColor, fogDensityCalc * Time.deltaTime);
        }

        currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, fogDensityCalc * Time.deltaTime);
        RenderSettings.fogDensity = currentFogDensity;
    }

    private void UpdateSkybox()
    {
        RenderSettings.skybox = isNight ? nightSkybox : daySkybox;
    }

    // -------------------- ���� ��ȯ --------------------
    private void UpdateLighting()
    {
        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(directionalLight.color,
                                                isNight ? nightLightColor : dayLightColor,
                                                Time.deltaTime * 2f);

            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity,
                                                    isNight ? nightLightIntensity : dayLightIntensity,
                                                    Time.deltaTime * 2f);
        }
    }

    // -------------------- ����Ʈ ���μ��� ��ȯ --------------------
    private void UpdatePostProcessing()
    {
        if (dayVolume != null && nightVolume != null)
        {
            float targetDay = isNight ? 0f : 1f;
            float targetNight = isNight ? 1f : 0f;

            dayVolume.weight = Mathf.Lerp(dayVolume.weight, targetDay, Time.deltaTime * 2f);
            nightVolume.weight = Mathf.Lerp(nightVolume.weight, targetNight, Time.deltaTime * 2f);
        }
    }

    // -------------------- ü�� ������ ���� --------------------
    private void ApplyColdDamage()
    {
        if (temperature < 0 && statusController != null)
        {
            float rawDamage = coldDamageRate * Mathf.Abs(temperature) * Time.deltaTime;
            float finalDamage = rawDamage;

            if (craftManual != null)
            {
                finalDamage = craftManual.CalculateColdDamageAfterProtection(rawDamage);
            }

            hpLossBuffer += finalDamage;

            if (hpLossBuffer >= 1f)
            {
                int intDamage = Mathf.FloorToInt(hpLossBuffer);
                statusController.DecreaseHP(intDamage);
                hpLossBuffer -= intDamage;
            }
        }
    }

    // -------------------- �ܺ� ���� �Լ� --------------------
    public void SetTime(float angleX)
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(angleX, currentRotation.y, currentRotation.z);
    }

    public void SetTemperature(float newTemp)
    {
        temperature = Mathf.Clamp(newTemp, minTemperature, maxTemperature);
    }

    public float GetTemperature()
    {
        return temperature;
    }


}
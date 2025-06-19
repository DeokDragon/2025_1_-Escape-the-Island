using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    
    public float currentTime
    {
        get { return transform.eulerAngles.x; }
    }
    // -------------------- 시간 --------------------
    [Header("시간 진행")]
    [SerializeField] private float secondPerRealTimeSecond = 60f;

    // -------------------- UI --------------------
    [Header("온도 UI")]
    [SerializeField] private Text temperatureText;
    [SerializeField] private RectTransform thermometerFill;
    [SerializeField] private float thermometerMaxHeight = 200f;

    // -------------------- 온도 --------------------
    [Header("온도 설정")]
    [SerializeField] private float temperature = 10f;
    [SerializeField] private float minTemperature = -10f;
    [SerializeField] private float maxTemperature = 25f;
    [SerializeField] private float temperatureChangeSpeed = 1f;
    [SerializeField] private float coldDamageRate = 1f;

    [Header("안개 설정")]
    [SerializeField] private float fogDensityCalc = 1f;
    [SerializeField] private float defaultDayFogDensity = 0.01f;
    [SerializeField] private float morningFogDensity = 0.05f;  // 아침 안개 밀도 추가
    [SerializeField] private float nightFogDensity = 0.1f;
    [SerializeField] private Color dayFogColor = Color.gray;
    [SerializeField] private Color morningFogColor = new Color(0.7f, 0.7f, 0.8f); // 아침 안개색 (연한 회색/파란빛)
    [SerializeField] private Color nightFogColor = Color.black;
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    // -------------------- 조명 --------------------
    [Header("조명 설정")]
    [SerializeField] private Light directionalLight;
    // playerFlashlight 관련 변수 제거
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private Color nightLightColor = new Color(0.2f, 0.2f, 0.4f);
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float nightLightIntensity = 0.1f;

    // -------------------- 포스트 프로세싱 --------------------
    [Header("포스트 프로세싱")]
    [SerializeField] private Volume dayVolume;
    [SerializeField] private Volume nightVolume;

    // -------------------- 내부 변수 --------------------
    private bool isNight = false;
    private float dayFogDensity;
    private float currentFogDensity;
    private float targetFogDensity;
    private float hpLossBuffer = 0f;

    private StatusController statusController;
    private Transform playerTransform;

    // -------------------- Craft 변수  --------------------
    private CraftManual craftManual;

    // -------------------- 초기 설정 --------------------
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

    // -------------------- 매 프레임 처리 --------------------
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

    // -------------------- 태양 회전 --------------------
    private void RotateSun()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);
    }

    // -------------------- 낮/밤 판별 --------------------
    private void UpdateDayNightState()
    {
        float angleX = transform.eulerAngles.x;
        isNight = angleX >= 170f && angleX < 340f;
    }

    // -------------------- 온도 변화 --------------------
    private void UpdateTemperature()
    {
        float targetTemp;

        // 동굴 안일 경우 무조건 영하로
        if (CaveStateManager.Instance != null && CaveStateManager.Instance.IsPlayerInsideCave)
        {
            targetTemp = -10f; // 원하시는 영하 값으로 설정 (-5도, -10도 등)
        }
        else
        {
            targetTemp = isNight ? minTemperature : maxTemperature;
        }

        temperature = Mathf.Lerp(temperature, targetTemp, temperatureChangeSpeed * Time.deltaTime);
    }

    private void UpdateTemperatureUI()
    {
        if (temperatureText != null)
            temperatureText.text = $"{temperature:F1}°C";

        if (thermometerFill != null)
        {
            float normalized = Mathf.InverseLerp(minTemperature, maxTemperature, temperature);
            float height = normalized * thermometerMaxHeight;
            thermometerFill.sizeDelta = new Vector2(thermometerFill.sizeDelta.x, height);
        }
    }

    // -------------------- 안개 조절 --------------------
    private void UpdateFog()
    {
        if (CaveStateManager.Instance != null && CaveStateManager.Instance.IsPlayerInsideCave)
        {
            // CaveEntranceTrigger가 설정한 값을 유지
            return;
        }

        float angleX = transform.eulerAngles.x;
        if (isNight)
        {
            targetFogDensity = nightFogDensity;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, nightFogColor, fogDensityCalc * Time.deltaTime);
        }
        else if (angleX >= 0f && angleX <= 60f)  // 아침
        {
            targetFogDensity = morningFogDensity;
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, morningFogColor, fogDensityCalc * Time.deltaTime);
        }
        else  // 낮
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

    // -------------------- 조명 전환 --------------------
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

    // -------------------- 포스트 프로세싱 전환 --------------------
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

    // -------------------- 체온 데미지 적용 --------------------
    private void ApplyColdDamage()
    {
        if (temperature < 0 && statusController != null)
        {
            float rawDamage = coldDamageRate * Mathf.Abs(temperature) * Time.deltaTime;
            float finalDamage = rawDamage;

            if (craftManual != null)
            {
                // 동굴 안, 밖 상관없이 갑옷 착용에 따른 데미지 감소 적용
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

    // -------------------- 외부 제어 함수 --------------------
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


    // 동굴안에서 사망시 화면 흐림현상 제거
    public void ForceOutsideReset()
    {
        RenderSettings.fogColor = dayFogColor;
        RenderSettings.fogDensity = defaultDayFogDensity;
        RenderSettings.skybox = daySkybox;
        isNight = false;
        SetTemperature(maxTemperature);
    }


// f4로 밤낮 설정하기
    public void ToggleDayNight()
    {
        float currentAngle = transform.eulerAngles.x;

        if (currentAngle >= 170f && currentAngle < 340f)
        {
            // 현재 밤 → 낮으로 변경
            SetTime(60f);   // 아침 각도
            
        }
        else
        {
            // 현재 낮 → 밤으로 변경
            SetTime(260f);  // 밤 각도
           
        }
    }

}
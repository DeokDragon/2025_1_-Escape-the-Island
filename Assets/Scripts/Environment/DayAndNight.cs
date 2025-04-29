using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond;
    [SerializeField] private Text temperatureText; // 화면에 표시될 온도 텍스트

    private bool isNight = false;

    [SerializeField] private float fogDensityCalc;
    [SerializeField] private float defaultDayFogDensity = 0.01f;
    [SerializeField] private float nightFogDensity = 0.1f;  // 밤의 안개 밀도
    private float dayFogDensity;
    private float currentFogDensity;
    private float targetFogDensity;

    // 온도 관련 변수
    [SerializeField] private float temperature = 10f;  // 현재 온도
    [SerializeField] private float coldDamageRate = 1f; // 추위에 의한 체력 감소율
    [SerializeField] private float minTemperature = -10f;  // 최저 온도
    [SerializeField] private float maxTemperature = 25f;  // 최고 온도
    [SerializeField] private float temperatureChangeSpeed = 1f; // 온도 변화 속도
    private float hpLossBuffer = 0f;


    private StatusController statusController;  // 플레이어 상태 관리
    private Transform playerTransform;  // 플레이어 Transform

    // Start is called before the first frame update
    void Start()
    {
        statusController = FindObjectOfType<StatusController>();

        // 플레이어 객체를 찾아서 Transform을 가져옵니다.
        playerTransform = GameObject.FindWithTag("Player").transform;  // 플레이어를 찾음

        dayFogDensity = defaultDayFogDensity;

        // 초기 설정
        currentFogDensity = dayFogDensity;
        targetFogDensity = dayFogDensity;
        RenderSettings.fogDensity = currentFogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        float angleX = transform.eulerAngles.x;
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        // 밤/낮 구분
        if (angleX >= 170f && angleX < 340f)
            isNight = true;
        else
            isNight = false;

        // 온도 변화 처리
        UpdateTemperature();

        if (temperatureText != null)
        {
            temperatureText.text = $"{temperature:F1}°C";
        }

        // 안개 밀도 조정
        if (!isNight) // 낮
        {
            targetFogDensity = dayFogDensity;
        }
        else // 밤
        {
            targetFogDensity = nightFogDensity;
        }

        // 안개 밀도 변화
        currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, fogDensityCalc * Time.deltaTime);
        RenderSettings.fogDensity = currentFogDensity;

        // 체온이 낮으면 체력 감소
        if (temperature < 0)
        {
            float damageAmount = coldDamageRate * Mathf.Abs(temperature) * Time.deltaTime;
            hpLossBuffer += damageAmount;

            if (hpLossBuffer >= 1f)
            {
                int intDamage = Mathf.FloorToInt(hpLossBuffer);
                statusController.DecreaseHP(intDamage);
                hpLossBuffer -= intDamage;
            }
        }
    }



    private void UpdateTemperature()
    {
        float angle = transform.eulerAngles.x;

        if (angle >= 170 && angle <= 340)  // 밤 (170도 ~ 340도 사이)
        {
            temperature = Mathf.Lerp(temperature, minTemperature, temperatureChangeSpeed * Time.deltaTime);
        }
        else  // 낮
        {
            temperature = Mathf.Lerp(temperature, maxTemperature, temperatureChangeSpeed * Time.deltaTime);
        }
    }

    public void SetTemperature(float newTemperature)
    {
        temperature = Mathf.Clamp(newTemperature, minTemperature, maxTemperature); // 체온을 최소/최대 범위로 제한
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

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond;
    [SerializeField] private Text temperatureText; //온도 텍스트

    private bool isNight = false;

    [SerializeField] private float fogDensityCalc;
    [SerializeField] private float defaultDayFogDensity = 0.01f;
    [SerializeField] private float nightFogDensity;
    private float dayFogDensity;
    private float currentFogDensity;

    // 온도 관련 변수
    [SerializeField] private float temperature = 10f;  // 초기 온도 (섭씨)
    [SerializeField] private float coldDamageRate = 1f; // 영하 온도에서 초당 체력 감소량
    [SerializeField] private float minTemperature = -10f;  // 밤에 최저 온도
    [SerializeField] private float maxTemperature = 25f;  // 낮에 최고 온도
    [SerializeField] private float temperatureChangeSpeed = 1f; // 온도 변화 속도

    private StatusController statusController;  // StatusController의 참조

    // Start is called before the first frame update
    void Start()
    {
        statusController = FindObjectOfType<StatusController>();
        // 낮의 기본 안개 밀도를 명확하게 설정
        dayFogDensity = defaultDayFogDensity;
        currentFogDensity = RenderSettings.fogDensity; // 현재 안개 밀도 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        if (transform.eulerAngles.x >= 170)
            isNight = true;
        else if (transform.eulerAngles.x >= 340)
            isNight = false;

        // 온도 변화 처리
        UpdateTemperature();

        if (temperatureText != null)
        {
            temperatureText.text = $"현재 온도: {temperature:F1}°C";
        }

        // 안개 관련 로직
        if (!isNight) // 낮일 때
        {
            if (currentFogDensity > dayFogDensity) // dayFogDensity보다 클 때만 감소
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                currentFogDensity = Mathf.Max(currentFogDensity, dayFogDensity); // dayFogDensity 이하로 떨어지지 않도록 보정
                RenderSettings.fogDensity = currentFogDensity;
            }
        }


        // 온도에 따른 체력 감소 처리
        if (temperature < 0)
        {
            // 영하일 때 체력 감소
            float damageAmount = coldDamageRate * Time.deltaTime;
            statusController.DecreaseHP((int)damageAmount); // StatusController의 체력 감소 메소드 호출
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


    // 외부에서 온도를 설정할 수 있는 메소드
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
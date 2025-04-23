using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{ 
 [SerializeField] private float secondPerRealTimeSecond;
[SerializeField] private Text temperatureText; // 온도 텍스트

private bool isNight = false;

[SerializeField] private float fogDensityCalc;
[SerializeField] private float defaultDayFogDensity = 0.01f;
[SerializeField] private float nightFogDensity = 0.1f;  // 밤의 최대 안개 밀도
private float dayFogDensity;
private float currentFogDensity;
private float targetFogDensity;

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
    dayFogDensity = defaultDayFogDensity;

    // 아침부터 시작한다고 가정하고 낮 안개 값으로 초기화
    currentFogDensity = dayFogDensity;
    targetFogDensity = dayFogDensity;
    RenderSettings.fogDensity = currentFogDensity;
}

// Update is called once per frame
void Update()
{
    float angleX = transform.eulerAngles.x;
    transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

    // 밤과 낮을 판별
    if (angleX >= 170f && angleX < 340f)
        isNight = true;
    else
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
        targetFogDensity = dayFogDensity;
    }
    else // 밤일 때
    {
        targetFogDensity = nightFogDensity;
    }

    // 안개 밀도 서서히 변화
    currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, fogDensityCalc * Time.deltaTime);
    RenderSettings.fogDensity = currentFogDensity;

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
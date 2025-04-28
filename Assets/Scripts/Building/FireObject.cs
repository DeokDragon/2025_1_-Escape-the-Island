using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : MonoBehaviour
{
   
    [SerializeField] private float fireRadius = 5f;  // 불의 영향을 미치는 반경
    [SerializeField] private float temperatureIncreaseRate = 0.3f;  // 불 근처에서 체온 증가 비율

    private DayAndNight dayAndNightScript;  // DayAndNight 스크립트 참조

    void Start()
    {
        // DayAndNight 스크립트 찾기
        dayAndNightScript = FindObjectOfType<DayAndNight>();
    }

    void Update()
    {
        // Fire 객체와 플레이어의 거리 계산
        Collider[] colliders = Physics.OverlapSphere(transform.position, fireRadius);

        foreach (Collider collider in colliders)
        {
            // 플레이어가 불의 영향 범위에 들어왔을 때
            if (collider.CompareTag("Player"))
            {
                // 플레이어와의 거리 계산
                float distanceToFire = Vector3.Distance(collider.transform.position, transform.position);

                // 불과의 거리 비례로 온도 증가량 계산
                float temperatureIncrease = Mathf.Lerp(0, temperatureIncreaseRate, 1 - (distanceToFire / fireRadius));

                // 온도 증가
                if (dayAndNightScript != null)
                {
                    dayAndNightScript.SetTemperature(dayAndNightScript.GetTemperature() + temperatureIncrease);
                }
            }
        }
    }
}


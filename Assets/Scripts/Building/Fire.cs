using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private string fireName; // 불 이름
    [SerializeField] private float fireRadius = 5f;

    [SerializeField]
    private int damage; // 불 데미지

    [SerializeField]
    private float damageTime;
    private float currentDamageTime;

    [SerializeField]
    private float durationTime;
    private float currentDurationTime;

    [SerializeField]
    private ParticleSystem ps_Flame;

    // 필요한 컴포넌트
    private StatusController thePlayerStatus;
    private DayAndNight dayAndNightScript; // DayAndNight 스크립트 참조

    // 상태 변수
    private bool isFire = true;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        dayAndNightScript = FindObjectOfType<DayAndNight>(); // DayAndNight 스크립트 찾기
        currentDurationTime = durationTime;
    }

    // Update는 매 프레임마다 호출됨
    void Update()
    {
        if (isFire)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        currentDurationTime -= Time.deltaTime;

        if (currentDurationTime > 0)
            currentDamageTime -= Time.deltaTime;

        if (currentDurationTime <= 0)
        {
            OFF();
        }
    }

    private void OFF()
    {
        ps_Flame.Stop();
        isFire = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFire && other.transform.CompareTag("Player"))
        {
            if (currentDamageTime <= 0)
            {
                other.GetComponent<Burn>().StartBurning();
                thePlayerStatus.DecreaseHP(damage);
                currentDamageTime = damageTime;

                // 불 근처에서 온도 증가
                if (dayAndNightScript != null)
                {
                    // 플레이어와 불 사이의 거리 계산
                    float distanceToFire = Vector3.Distance(other.transform.position, transform.position);

                    // 거리 값이 fireRadius를 초과하면 0으로 처리
                    float clampedDistance = Mathf.Min(distanceToFire, fireRadius);

                    // 불과의 거리 비례로 온도 증가량 계산
                    // 거리가 가까울수록 비율이 1에 가까워지도록 함
                    float temperatureIncrease = Mathf.Lerp(0, 0.3f, 1 - (clampedDistance / fireRadius));

                    // 온도 증가
                    dayAndNightScript.SetTemperature(dayAndNightScript.GetTemperature() + temperatureIncrease); // 불 근처에서 온도 증가
                }
            }
        }
    }
}

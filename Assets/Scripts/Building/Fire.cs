using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private string fireName; // �� �̸�
    [SerializeField] private float fireRadius = 5f;

    [SerializeField]
    private int damage; // �� ������

    [SerializeField]
    private float damageTime;
    private float currentDamageTime;

    [SerializeField]
    private float durationTime;
    private float currentDurationTime;

    [SerializeField]
    private ParticleSystem ps_Flame;

    // �ʿ��� ������Ʈ
    private StatusController thePlayerStatus;
    private DayAndNight dayAndNightScript; // DayAndNight ��ũ��Ʈ ����

    // ���� ����
    private bool isFire = true;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        dayAndNightScript = FindObjectOfType<DayAndNight>(); // DayAndNight ��ũ��Ʈ ã��
        currentDurationTime = durationTime;
    }

    // Update�� �� �����Ӹ��� ȣ���
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

                // �� ��ó���� �µ� ����
                if (dayAndNightScript != null)
                {
                    // �÷��̾�� �� ������ �Ÿ� ���
                    float distanceToFire = Vector3.Distance(other.transform.position, transform.position);

                    // �Ÿ� ���� fireRadius�� �ʰ��ϸ� 0���� ó��
                    float clampedDistance = Mathf.Min(distanceToFire, fireRadius);

                    // �Ұ��� �Ÿ� ��ʷ� �µ� ������ ���
                    // �Ÿ��� �������� ������ 1�� ����������� ��
                    float temperatureIncrease = Mathf.Lerp(0, 0.3f, 1 - (clampedDistance / fireRadius));

                    // �µ� ����
                    dayAndNightScript.SetTemperature(dayAndNightScript.GetTemperature() + temperatureIncrease); // �� ��ó���� �µ� ����
                }
            }
        }
    }
}

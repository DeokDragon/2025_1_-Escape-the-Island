using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // ü��
    [SerializeField] private int hp;
    private int currentHp;

    // ���¹̳�
    [SerializeField] private int sp;
    private int currentSp;
    [SerializeField] private int spIncreaseSpeed;
    [SerializeField] private int spRechargeTime;
    private int currentSpRechargeTime;
    private bool spUsed;

    // �����
    [SerializeField] private int hungry;
    private int currentHungry;
    [SerializeField] private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // �񸶸�
    [SerializeField] private int thirsty;
    private int currentThirsty;
    [SerializeField] private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // UI ������
    [SerializeField] private Image[] images_Gauge;
    private const int HP = 0, SP = 1, HUNGRY = 2, THIRSTY = 3;

    void Start()
    {
        currentHp = hp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
    }

    void Update()
    {
        Hungry();
        Thirsty();
        GaugeUpdate();
        SPRechargeTime();
        SPRecover();

        // ����� �Ǵ� �񸶸��� 0�̸� ���¹̳��� 0���� ����
        if (currentHungry == 0 || currentThirsty == 0)
        {
            currentSp = 0;
        }
    }

    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    private void SPRecover()
    {
        if (!spUsed && currentSp < sp && currentThirsty > 0) // �񸶸��� 0�̸� ȸ�� �Ұ�
        {
            currentSp += spIncreaseSpeed;
        }
    }

    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime < hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime < thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
    }

    public void IncreaseHP(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, hp);
    }

    public void DecreaseHP(int amount)
    {
        currentHp -= amount;

        if (currentHp <= 0)
        {
            Debug.Log(" ĳ������ ü���� 0�� �Ǿ����ϴ�!");
        }
    }

    public void IncreaseHungry(int amount)
    {
        currentHungry = Mathf.Min(currentHungry + amount, hungry);
    }

    public void DecreaseHungry(int amount)
    {
        currentHungry = Mathf.Max(currentHungry - amount, 0);
    }

    public void IncreaseThirsty(int amount)
    {
        currentThirsty = Mathf.Min(currentThirsty + amount, thirsty);
    }

    public void DecreaseThirsty(int amount)
    {
        currentThirsty = Mathf.Max(currentThirsty - amount, 0);
    }

    public void DecreaseStamina(int amount)
    {
        spUsed = true;
        currentSpRechargeTime = 0;
        currentSp = Mathf.Max(currentSp - amount, 0);
    }

    public int GetCurrentSP()
    {
        return currentSp;
    }
}

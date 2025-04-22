using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField] private int hp;
    private int currentHp;

    // 스태미나
    [SerializeField] private int sp;
    private int currentSp;
    [SerializeField] private int spIncreaseSpeed;
    [SerializeField] private int spRechargeTime;
    private int currentSpRechargeTime;
    private bool spUsed;

    // 배고픔
    [SerializeField] private int hungry;
    private int currentHungry;
    [SerializeField] private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 목마름
    [SerializeField] private int thirsty;
    private int currentThirsty;
    [SerializeField] private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // UI 게이지
    [SerializeField] private Image[] images_Gauge;
    private const int HP = 0, SP = 1, HUNGRY = 2, THIRSTY = 3;

    // 소리 효과
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    bool hasHit = false;



public int GetCurrentStamina()
    {
        return currentSp;
    }

    public int GetCurrentHunger()
    {
        return currentHungry;
    }

    public int GetCurrentThirst()
    {
        return currentThirsty;
    }

    public void SetStatus(float hp, float sp, float hunger, float thirst)
    {
        currentHp = (int)hp;
        currentSp = (int)sp;
        currentHungry = (int)hunger;
        currentThirsty = (int)thirst;

        GaugeUpdate(); 
    }



    void Start()
    {
        currentHp = hp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Hungry();
        Thirsty();
        GaugeUpdate();
        SPRechargeTime();
        SPRecover();

        // 배고픔 또는 목마름이 0이면 스태미나를 0으로 설정
        if (currentHungry == 0 || currentThirsty == 0)
        {
            currentSp = 0;
        }
    }

    // 스태미나 회복 대기 시간
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

    // 스태미나 회복
    private void SPRecover()
    {
        if (!spUsed && currentSp < sp && currentThirsty > 0) // 목마름이 0이면 회복 불가
        {
            currentSp += spIncreaseSpeed;
        }
    }

    // 배고픔 감소
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

    // 목마름 감소
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

    // UI 게이지 업데이트
    private void GaugeUpdate()
    {
        float hpRatio = (float)currentHp / hp;
        float spRatio = (float)currentSp / sp;
        float hungryRatio = (float)currentHungry / hungry;
        float thirstyRatio = (float)currentThirsty / thirsty;

        



        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
    }

    // 체력 증가
    public void IncreaseHP(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, hp);
    }

    // 체력 감소
    public void DecreaseHP(int amount, GameObject attacker = null)
    {
        currentHp -= amount;

        // 곰한테 맞았을 때만 피격 사운드 재생
        if (attacker != null && attacker.CompareTag("Bear"))
        {
            PlayHitEffects();
        }

        if (currentHp <= 0)
        {
            Debug.Log("캐릭터의 체력이 0이 되었습니다!");

            RespawnManager respawn = FindObjectOfType<RespawnManager>();
            if (respawn != null)
            {
                respawn.Respawn();
            }

            // 필요 시 추가로 플레이어 움직임 잠금 등 처리 가능
        }
    }

    // 피격 효과
    private void PlayHitEffects()
    {
        PlayHitSound();
        // PlayDamageAnimation(); // 필요 시 애니메이션 추가
    }

    // 피격 소리 재생
    public void PlayHitSound()
    {
        if (audioSource != null && hitSound != null && !audioSource.isPlaying) // 이미 소리가 재생 중이지 않으면
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // 배고픔 증가
    public void IncreaseHungry(int amount)
    {
        currentHungry = Mathf.Min(currentHungry + amount, hungry);
    }

    // 배고픔 감소
    public void DecreaseHungry(int amount)
    {
        currentHungry = Mathf.Max(currentHungry - amount, 0);
    }

    // 목마름 증가
    public void IncreaseThirsty(int amount)
    {
        currentThirsty = Mathf.Min(currentThirsty + amount, thirsty);
    }

    // 목마름 감소
    public void DecreaseThirsty(int amount)
    {
        currentThirsty = Mathf.Max(currentThirsty - amount, 0);
    }

    // 스태미나 감소
    public void DecreaseStamina(int amount)
    {
        spUsed = true;
        currentSpRechargeTime = 0;
        currentSp = Mathf.Max(currentSp - amount, 0);
    }

    // 현재 스태미나 반환
    public int GetCurrentSP()
    {
        return currentSp;
    }

    // 현재 체력 반환 (필요할 수 있어서 추가)
    public int GetCurrentHP()
    {
        return currentHp;
    }
}
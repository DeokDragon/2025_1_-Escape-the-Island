using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{

    private bool isBurning = false;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float damageTime;
    private float currentDamageTime;

    [SerializeField]
    private float durationTime;
    private float currentDurationTime;

    [SerializeField]
    private GameObject flame_prefabs; // 불 붙으면 프리팹 생성
    private GameObject go_tempFlame;

    public void StartBurning()
    {
       

        if (!isBurning)
        {
            go_tempFlame = Instantiate(flame_prefabs, transform.position, Quaternion.Euler(new Vector3(-90, 0f, 0f)));
            go_tempFlame.transform.SetParent(transform);
        }
        isBurning = true;
        currentDurationTime = durationTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBurning)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        if (isBurning)
        {
            currentDurationTime -= Time.deltaTime;

            if (currentDamageTime > 0)
                currentDamageTime -= Time.deltaTime;

            if (currentDamageTime <= 0)
            {
                Damage();
            }
            if (currentDurationTime <= 0)
            {
                Off();
            }
        }
    }

    private void Damage()
    {
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHP(damage);

        StatusController status = GetComponent<StatusController>();
        if (status != null)
        {
            status.DecreaseHP(damage);
        }
        else
        {
            Debug.LogError(" Burn.cs: StatusController가 없음! (데미지 못 줌)");
        }
    }

    public void Off()
    {
        isBurning = false;

        if (go_tempFlame != null)
            Destroy(go_tempFlame);
    }
}

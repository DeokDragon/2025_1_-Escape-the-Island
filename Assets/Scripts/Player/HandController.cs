using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{

    public static bool isActivate = true;

    private void Start()
    {
        WeaponManager1.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager1.currentWeaponAnim = currentCloseWeapon.anim;
    }


    // Update is called once per frame
    void Update()
    {
       // if (isActivate && !thePlayerController.IsCrouch)
          // TryAttack();
    }


    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                // AxeAttack 스크립트 불러오기
                AxeAttack axeAttack = GetComponent<AxeAttack>();
                if (axeAttack != null)
                {
                    axeAttack.StartSwing();
                }
                else
                {
                    Debug.LogWarning("AxeAttack 스크립트가 없습니다!");
                }

                isSwing = false;
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    // 활성화 여부.
    public static bool isActivate = false;

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
            TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if (hitInfo.transform.tag == "Grass")
                {
                    SoundManager.instance.PlaySE("HitGrass");
                    hitInfo.transform.GetComponent<Grass>().Damage();
                }
                else if (hitInfo.transform.tag == "Tree")
                {
                    SoundManager.instance.PlaySE("HitTree");
                    hitInfo.transform.GetComponent<TreeComponent>().Chop(hitInfo.point, transform.eulerAngles.y);
                }
                else if (hitInfo.transform.tag == "Twig")
                {
                    SoundManager.instance.PlaySE("HitGrass");
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
                }
                else if (hitInfo.transform.tag == "Bear")
                {
                    SoundManager.instance.PlaySE("HitBear"); // 사운드 등록돼 있을 경우만
                    hitInfo.transform.GetComponent<BearHealth>().TakeDamage(50);
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


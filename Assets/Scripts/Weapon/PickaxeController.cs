using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{ // 활성화 여부.
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
                if(hitInfo.transform.tag == "Rock")
                {
                    SoundManager.instance.PlaySE("HitRock");
                    hitInfo.transform.GetComponent<Rock>().Mining();
                }
                else if (hitInfo.transform.tag == "Twig")
                {
                    SoundManager.instance.PlaySE("HitGrass");
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
                }
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
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

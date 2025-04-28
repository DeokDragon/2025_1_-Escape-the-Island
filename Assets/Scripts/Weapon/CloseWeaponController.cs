using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CloseWeaponController : MonoBehaviour
//미완성 클래스
{
    //스테이터스 컨트롤러 참조
    protected StatusController theStatusController;


    // 현재 장착된 Hand형 타입 무기.
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중??
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]


    //필요한 컴포넌트
    protected PlayerController thePlayerController;

    void Start()
    {
        thePlayerController = FindObjectOfType<PlayerController>();
        theStatusController = FindObjectOfType<StatusController>();//참조
    }

    protected void TryAttack()
    {
        if (!Inventory.inventoryActivated && !thePlayerController.IsCrouch) // 쪼그림 체크
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (thePlayerController.IsCrouch) // << 추가!
                    return; // 앉아있으면 공격 안 함

                if (!isAttack && theStatusController.GetCurrentStamina() >= 2500)
                {
                    if (CheckObject())
                    {
                        if (currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                        {                     
                            StartCoroutine(AttackCoroutine("Chop", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                            return;
                        }
                    }

                    StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
                }
            }
        }
    }

    protected IEnumerator AttackCoroutine(string swingType, float _delayA, float _delayB, float _delayC)
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger(swingType);

        // 공격 시작 전 스태미나 감소
        if (theStatusController != null)
        {
            theStatusController.DecreaseStamina(1500); // 원하는 수치로 조정
        }

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delayC - _delayA - _delayB);

        isAttack = false;
    }


    // 미완성 = 추상 코루틴.
    protected abstract IEnumerator HitCoroutine();


    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true;
        }
        return false;
    }

    // 완성 함수이지만, 추가 편집한 함수.
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager1.currentWeapon != null)
            WeaponManager1.currentWeapon.gameObject.SetActive(false);

        currentCloseWeapon = _closeWeapon;
        WeaponManager1.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager1.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}

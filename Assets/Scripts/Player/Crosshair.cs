using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator animator;




    // 크로스 헤어 비활성화를 위한 부모 객체.
    [SerializeField]
    private GameObject go_CrosshairHUD;



    public void WalkingAnimation(bool _flag)
    {
        
            WeaponManager1.currentWeaponAnim.SetBool("Walk", _flag);
            animator.SetBool("Walking", _flag);
        

    }

    public void RunningAnimation(bool _flag)
    {
        
        
            WeaponManager1.currentWeaponAnim.SetBool("Run", _flag);
            animator.SetBool("Running", _flag);
        
    }

    public void JumpingAnimation(bool _flag)
    {
         
        
            animator.SetBool("Running", _flag);
        

    }

    public void CrouchingAnimation(bool _flag)
    {
        
        
            animator.SetBool("Crouching", _flag);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{

    public string closeWeaponName;  //��Ŭ, �Ǽ� ����


    //웨폰 유형.
    public bool isHand;
    public bool isAxe;
    public bool isPickaxe;

    public float range; // ���� ����
    public int damage;
    public float workSpeed;
    public float attackDelay; //���� �����
    public float attackDelayA; //���� Ȱ��ȭ ����
    public float attackDelayB; // ���� ��Ȱ��ȭ ����;

    public float workDelay; //작업 딜레이
    public float workDelayA; //작업 활성화
    public float workDelayB; //작업 비활성화

    public Animator anim; //�ִ�
    
   
}

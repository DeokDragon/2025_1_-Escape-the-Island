using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // 풀 체력 (보통은 1)
    [SerializeField]
    private int hp;

    // 이펙트 제거 시간.
    [SerializeField]
    private float destroyTime;

    // 폭발력 세기
    [SerializeField]
    private float force;

    // 타격 효과
    [SerializeField]
    private GameObject go_hit_effect_prefab;


    [SerializeField]
    private Item item_leaf;
    [SerializeField]
    private int leafCount;
    private Inventory theInven;

    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColiiders;


    [SerializeField]
    private string hit_sound;

    private bool isDestroyed = false;
    // Use this for initialization
    void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColiiders = transform.GetComponentsInChildren<BoxCollider>();
    }


    public void Damage()
    {
        if (isDestroyed) return;  // 이미 파괴된 경우 중복 실행 방지

        hp--;

        Hit();

        if (hp <= 0)
        {
            Destruction();
        }
    }

    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_sound);

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(clone, destroyTime);
    }

    private void Destruction()
    {
        if (isDestroyed) return;  // 이미 실행되었으면 다시 실행하지 않음
        isDestroyed = true;

        theInven.AcquireItem(item_leaf, leafCount);

        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
        }

        StartCoroutine(EnableCollidersWithDelay());

        Destroy(this.gameObject, destroyTime);
    }

    private IEnumerator EnableCollidersWithDelay()
    {
        yield return null;  // 한 프레임 기다림

        for (int i = 0; i < boxColiiders.Length; i++)
        {
            boxColiiders[i].enabled = true;
        }
    }
}

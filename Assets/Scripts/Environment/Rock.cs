using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("Rock Stats")]
    [SerializeField] private int hp;
    [SerializeField] private float destroyTime;
    [SerializeField] private float objectDestroyTime;

    [Header("Components")]
    [SerializeField] private SphereCollider col;

    [Header("Prefabs")]
    [SerializeField] private GameObject go_rock;
    [SerializeField] private GameObject go_debris;
    [SerializeField] private GameObject go_effect_prefabs;
    [SerializeField] private GameObject go_rock_item_prefab;

    [Header("Drop Settings")]
    [SerializeField] private int count;

    [Header("Sound Names")]
    [SerializeField] private string strike_Sound = "Pickaxe_Strike";
    [SerializeField] private string destroy_Sound = "Rock_Destroy";

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);

        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;

        Vector3 spawnPosition = go_rock != null ? go_rock.transform.position : transform.position;

        for (int i = 0; i <= count; i++)
        {
            Instantiate(go_rock_item_prefab, spawnPosition, Quaternion.identity);
        }

        if (go_rock != null)
            Destroy(go_rock);

        if (go_debris != null)
        {
            go_debris.SetActive(true);
            Destroy(go_debris, destroyTime);
        }

        Destroy(gameObject, objectDestroyTime);
    }

}

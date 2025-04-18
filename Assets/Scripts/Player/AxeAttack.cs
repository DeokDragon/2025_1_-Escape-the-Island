using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    public Camera playerCamera;
    public float attackRange = 3f;
    public int damage = 50;

    public bool isSwinging = false;

    void Update()
    {
        if (isSwinging)
        {
            PerformAttack();
            isSwinging = false; // 한 번만 공격
        }
    }

    public void PerformAttack()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            BearHealth bear = hit.collider.GetComponent<BearHealth>();
            if (bear != null)
            {
                bear.TakeDamage(damage);
                Debug.Log("Bear hit!");
            }
            else
            {
                Debug.Log("Hit something else: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }

    public void StartSwing()
    {
        isSwinging = true;
    }
}

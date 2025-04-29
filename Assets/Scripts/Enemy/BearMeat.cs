using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMeat : MonoBehaviour
{
    public Item bearMeatItem;
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (bearMeatItem == null)
            {
                Debug.LogError("������ �����Ͱ� �����ϴ�! bearMeatItem �� null �Դϴ�!");
                return;
            }

            Debug.Log("E Ű �Է� Ȯ�ε�, ������ ȹ�� �õ�!");
            Inventory.instance.AcquireItem(bearMeatItem);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}

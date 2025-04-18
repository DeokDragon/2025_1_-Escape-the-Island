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
                Debug.LogError("아이템 데이터가 없습니다! bearMeatItem 이 null 입니다!");
                return;
            }

            Debug.Log("E 키 입력 확인됨, 아이템 획득 시도!");
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

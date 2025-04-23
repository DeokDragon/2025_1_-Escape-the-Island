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
                return;
            }
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

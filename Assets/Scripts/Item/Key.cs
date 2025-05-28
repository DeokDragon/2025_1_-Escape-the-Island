using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
   
    public float interactDistance = 3f;
  
    private bool isPlayerLooking = false;

    void Update()
    {
        isPlayerLooking = false;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isPlayerLooking = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.hasKey = true;
                    Destroy(gameObject);
                }
            }
        }

    }
}

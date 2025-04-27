using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObject : MonoBehaviour
{

    void OnDestroy()
    {
        
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.UnregisterSpawn(gameObject);
        }
    }
}

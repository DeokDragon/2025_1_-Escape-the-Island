using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnDistance = 2f; // 카메라 앞 몇 미터에 소환할지
    public KeyCode spawnKey = KeyCode.T;

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnInFrontOfCamera();
        }
    }

    void SpawnInFrontOfCamera()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("소환할 프리팹이 설정되지 않았습니다!");
            return;
        }

        Transform cam = Camera.main.transform;
        Vector3 spawnPosition = cam.position + cam.forward * spawnDistance;
        Quaternion spawnRotation = Quaternion.LookRotation(cam.forward); // 카메라가 바라보는 방향

        Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        Debug.Log("프리팹이 시야 앞에 소환되었습니다!");
    }
}
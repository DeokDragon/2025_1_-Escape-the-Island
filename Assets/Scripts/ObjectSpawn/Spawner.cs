using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float respawnDelay = 3f;

    private GameObject currentSpawnedObject;
    private bool isRespawning = false;

    private void Start()
    {
        SpawnManager.Instance.RegisterSpawner(this);
        StartRespawn();  // 게임 시작할 때 바로 생성
    }

    public bool CanRespawn()
    {
        return currentSpawnedObject == null && !isRespawning;
    }

    public void StartRespawn()
    {
        if (CanRespawn())
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        if (SpawnManager.Instance.CanSpawn())
        {
            currentSpawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
            SpawnManager.Instance.RegisterSpawn(currentSpawnedObject);
        }

        isRespawning = false;
    }
}

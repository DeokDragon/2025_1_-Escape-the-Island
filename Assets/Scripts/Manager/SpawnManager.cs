using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public int maxSpawnedObjects = 10;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Spawner> spawners = new List<Spawner>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterSpawner(Spawner spawner)
    {
        spawners.Add(spawner);
    }

    public void RegisterSpawn(GameObject obj)
    {
        spawnedObjects.Add(obj);
    }

    public void UnregisterSpawn(GameObject obj)
    {
        spawnedObjects.Remove(obj);
        TryRespawn();
    }

    public bool CanSpawn()
    {
        return spawnedObjects.Count < maxSpawnedObjects;
    }

    private void TryRespawn()
    {
        if (!CanSpawn()) return;

        // 랜덤한 스포너 골라서 리스폰 요청
        var availableSpawners = spawners.FindAll(s => s.CanRespawn());
        if (availableSpawners.Count > 0)
        {
            var randomSpawner = availableSpawners[Random.Range(0, availableSpawners.Count)];
            randomSpawner.StartRespawn();
        }
    }
}

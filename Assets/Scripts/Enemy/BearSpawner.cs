using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BearSpawner : MonoBehaviour
{
    public GameObject bearPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 43200f; // 12½Ã°£
    public int maxBearsAtOnce = 3;

    private float spawnTimer = 0f;
    private List<GameObject> spawnedBears = new List<GameObject>();

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;

            // Á×Àº °õ Á¦°Å
            spawnedBears.RemoveAll(b => b == null);

            int bearsToSpawn = maxBearsAtOnce - spawnedBears.Count;
            for (int i = 0; i < bearsToSpawn; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject bear = Instantiate(bearPrefab, spawnPos, Quaternion.identity);
                spawnedBears.Add(bear);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            transform.position.x + Random.Range(-spawnRadius, spawnRadius),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRadius, spawnRadius)
        );
    }
}
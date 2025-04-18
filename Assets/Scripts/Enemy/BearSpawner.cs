using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSpawner : MonoBehaviour
{
    public GameObject bearPrefab;  // 곰 프리팹
    public float spawnRadius = 10f; // 스폰 반경
    public float spawnInterval = 30f; // 곰이 스폰되는 간격 (30초)
    public int maxBearsAtOnce = 3; // 한번에 최대 생성될 곰 수

    private List<Vector3> spawnedBearPositions = new List<Vector3>(); // 이미 스폰된 곰들의 위치 리스트

    private void Start()
    {
        // 일정 시간 간격으로 곰 스폰 함수 호출
        StartCoroutine(SpawnBearsAtIntervals());
    }

    private IEnumerator SpawnBearsAtIntervals()
    {
        while (true)
        {
            // 한번에 최대 maxBearsAtOnce 마리의 곰을 스폰
            int bearsSpawned = 0;
            while (bearsSpawned < maxBearsAtOnce)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                if (spawnPosition != Vector3.zero) // 유효한 위치일 경우
                {
                    Instantiate(bearPrefab, spawnPosition, Quaternion.identity);
                    spawnedBearPositions.Add(spawnPosition); // 곰의 위치를 저장
                    bearsSpawned++;
                }
            }

            // 30초 간격으로 반복
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 스폰할 위치를 랜덤으로 계산
        Vector3 randomPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRadius, spawnRadius),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRadius, spawnRadius)
        );

        // 이미 해당 위치 근처에 곰이 있으면 위치를 다시 계산
        foreach (Vector3 bearPosition in spawnedBearPositions)
        {
            if (Vector3.Distance(bearPosition, randomPosition) < spawnRadius) // 근처에 다른 곰이 있으면
            {
                return Vector3.zero; // 유효하지 않은 위치, 다시 시도하도록
            }
        }

        return randomPosition; // 유효한 위치
    }
}
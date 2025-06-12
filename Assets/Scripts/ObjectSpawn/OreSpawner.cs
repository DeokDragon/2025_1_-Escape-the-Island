using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OreData
{
    public GameObject orePrefab;
    public int countPerCave;
}

public class OreSpawner : MonoBehaviour
{
    public List<OreData> ores; // 각 원석 프리팹과 개수 설정
    public LayerMask groundMask; // 바닥 확인용 레이어 마스크

    void Start()
    {
        SpawnOresInCaves();
    }

    void SpawnOresInCaves()
    {
        GameObject[] caves = GameObject.FindGameObjectsWithTag("Cave");

        foreach (GameObject cave in caves)
        {
            Collider caveCollider = cave.GetComponent<Collider>();

            if (caveCollider == null)
            {
                Debug.LogWarning("Cave에 Collider가 없습니다!");
                continue;
            }

            foreach (var oreData in ores)
            {
                int spawned = 0;
                int maxAttempts = oreData.countPerCave * 5;

                while (spawned < oreData.countPerCave && maxAttempts-- > 0)
                {
                    Vector3 randomPosition = GetRandomPointInBounds(caveCollider.bounds);

                    if (IsWalkable(randomPosition, out Vector3 spawnPosition))
                    {
                        Instantiate(oreData.orePrefab, spawnPosition + Vector3.up * 0.4f, Quaternion.identity);
                        spawned++;
                    }
                }
            }
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.max.y + 20f, // 기존보다 더 높은 위치에서 시작
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    bool IsWalkable(Vector3 position, out Vector3 spawnPosition)
    {
        RaycastHit hit;
        float rayLength = 100f; // 100유닛까지 바닥 탐색

        bool result = Physics.Raycast(position, Vector3.down, out hit, rayLength, groundMask);

        // 디버그 선
        Debug.DrawRay(position, Vector3.down * rayLength, result ? Color.green : Color.red, 5f);

        spawnPosition = result ? hit.point : Vector3.zero;
        return result;
    }
}

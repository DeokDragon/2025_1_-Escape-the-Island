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
    public List<OreData> ores; // �� ���� �����հ� ���� ����
    public LayerMask groundMask; // �ٴ� Ȯ�ο� ���̾� ����ũ

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
                Debug.LogWarning("Cave�� Collider�� �����ϴ�!");
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
            bounds.max.y + 20f, // �������� �� ���� ��ġ���� ����
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    bool IsWalkable(Vector3 position, out Vector3 spawnPosition)
    {
        RaycastHit hit;
        float rayLength = 100f; // 100���ֱ��� �ٴ� Ž��

        bool result = Physics.Raycast(position, Vector3.down, out hit, rayLength, groundMask);

        // ����� ��
        Debug.DrawRay(position, Vector3.down * rayLength, result ? Color.green : Color.red, 5f);

        spawnPosition = result ? hit.point : Vector3.zero;
        return result;
    }
}

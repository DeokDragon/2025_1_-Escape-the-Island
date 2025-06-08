using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    public GameObject[] orePrefabs; // Iron, Rock, Coal, Diamond Prefabs
    public int oreCountPerCave = 20; // �������� ������ ���� ��

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

            for (int i = 0; i < oreCountPerCave; i++)
            {
                Vector3 randomPosition = GetRandomPointInBounds(caveCollider.bounds);

                // NavMesh�� ����� ��� �̵� �������� Ȯ��
                if (IsWalkable(randomPosition))
                {
                    GameObject orePrefab = orePrefabs[Random.Range(0, orePrefabs.Length)];
                    Instantiate(orePrefab, randomPosition, Quaternion.identity);
                }
                else
                {
                    i--; // ���������� �ٽ� �õ�
                }
            }
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    bool IsWalkable(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            // Layer�� �Ǵ��ϰų�, Ư�� �±׷� �ٴ����� Ȯ��
            return hit.collider.CompareTag("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Walkable");
        }
        return false;
    }
}

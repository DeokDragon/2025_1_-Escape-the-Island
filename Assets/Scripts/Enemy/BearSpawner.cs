using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSpawner : MonoBehaviour
{
    public GameObject bearPrefab;  // �� ������
    public float spawnRadius = 10f; // ���� �ݰ�
    public float spawnInterval = 30f; // ���� �����Ǵ� ���� (30��)
    public int maxBearsAtOnce = 3; // �ѹ��� �ִ� ������ �� ��

    private List<Vector3> spawnedBearPositions = new List<Vector3>(); // �̹� ������ ������ ��ġ ����Ʈ

    private void Start()
    {
        // ���� �ð� �������� �� ���� �Լ� ȣ��
        StartCoroutine(SpawnBearsAtIntervals());
    }

    private IEnumerator SpawnBearsAtIntervals()
    {
        while (true)
        {
            // �ѹ��� �ִ� maxBearsAtOnce ������ ���� ����
            int bearsSpawned = 0;
            while (bearsSpawned < maxBearsAtOnce)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                if (spawnPosition != Vector3.zero) // ��ȿ�� ��ġ�� ���
                {
                    Instantiate(bearPrefab, spawnPosition, Quaternion.identity);
                    spawnedBearPositions.Add(spawnPosition); // ���� ��ġ�� ����
                    bearsSpawned++;
                }
            }

            // 30�� �������� �ݺ�
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // ������ ��ġ�� �������� ���
        Vector3 randomPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRadius, spawnRadius),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRadius, spawnRadius)
        );

        // �̹� �ش� ��ġ ��ó�� ���� ������ ��ġ�� �ٽ� ���
        foreach (Vector3 bearPosition in spawnedBearPositions)
        {
            if (Vector3.Distance(bearPosition, randomPosition) < spawnRadius) // ��ó�� �ٸ� ���� ������
            {
                return Vector3.zero; // ��ȿ���� ���� ��ġ, �ٽ� �õ��ϵ���
            }
        }

        return randomPosition; // ��ȿ�� ��ġ
    }
}
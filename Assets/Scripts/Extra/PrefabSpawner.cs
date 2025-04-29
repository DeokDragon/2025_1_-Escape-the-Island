using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnDistance = 2f; // ī�޶� �� �� ���Ϳ� ��ȯ����
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
            Debug.LogWarning("��ȯ�� �������� �������� �ʾҽ��ϴ�!");
            return;
        }

        Transform cam = Camera.main.transform;
        Vector3 spawnPosition = cam.position + cam.forward * spawnDistance;
        Quaternion spawnRotation = Quaternion.LookRotation(cam.forward); // ī�޶� �ٶ󺸴� ����

        Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        Debug.Log("�������� �þ� �տ� ��ȯ�Ǿ����ϴ�!");
    }
}
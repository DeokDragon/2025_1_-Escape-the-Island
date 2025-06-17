using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipper : MonoBehaviour
{
    public Transform weaponParent; // ���� ���� ��ġ (ī�޶� �ڽ� ������Ʈ ��õ)

    [Header("�����յ�")]
    public GameObject rockAxePrefab;
    public GameObject ironAxePrefab;
    public GameObject diamondAxePrefab;
    public GameObject alloyAxePrefab; //�̽���

    public GameObject rockPickaxePrefab;
    public GameObject ironPickaxePrefab;
    public GameObject diamondPickaxePrefab;
    public GameObject alloyPickaxePrefab; //�̰͵� �̽���

    private GameObject currentWeapon;

    public void EquipWeaponByName(string weaponName)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        GameObject prefabToSpawn = GetPrefabByName(weaponName);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"�������� ã�� �� �����ϴ�: {weaponName}");
            return;
        }

        currentWeapon = Instantiate(prefabToSpawn, weaponParent);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // ���� ��ũ��Ʈ �ʱ�ȭ �� �ʿ��� ó�� �߰� ����
    }

    private GameObject GetPrefabByName(string name)
    {
        return name switch
        {
            "RockAxe" => rockAxePrefab,
            "IronAxe" => ironAxePrefab,
            "DiamondAxe" => diamondAxePrefab,
            "AlloyAxe" => alloyAxePrefab, //�̽�������
            "RockPickaxe" => rockPickaxePrefab,
            "IronPickaxe" => ironPickaxePrefab,
            "DiamondPickaxe" => diamondPickaxePrefab,
            "AlloyPickaxe" => alloyPickaxePrefab, //�̽������
            _ => null
        };
    }
}
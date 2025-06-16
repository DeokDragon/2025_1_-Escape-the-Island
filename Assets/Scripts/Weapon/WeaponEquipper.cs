using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipper : MonoBehaviour
{
    public Transform weaponParent; // 무기 장착 위치 (카메라 자식 오브젝트 추천)

    [Header("프리팹들")]
    public GameObject rockAxePrefab;
    public GameObject ironAxePrefab;
    public GameObject diamondAxePrefab;
    public GameObject alloyAxePrefab; //미스릴

    public GameObject rockPickaxePrefab;
    public GameObject ironPickaxePrefab;
    public GameObject diamondPickaxePrefab;
    public GameObject alloyPickaxePrefab; //이것도 미스릴

    private GameObject currentWeapon;

    public void EquipWeaponByName(string weaponName)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        GameObject prefabToSpawn = GetPrefabByName(weaponName);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {weaponName}");
            return;
        }

        currentWeapon = Instantiate(prefabToSpawn, weaponParent);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // 무기 스크립트 초기화 등 필요한 처리 추가 가능
    }

    private GameObject GetPrefabByName(string name)
    {
        return name switch
        {
            "RockAxe" => rockAxePrefab,
            "IronAxe" => ironAxePrefab,
            "DiamondAxe" => diamondAxePrefab,
            "AlloyAxe" => alloyAxePrefab, //미스릴도끼
            "RockPickaxe" => rockPickaxePrefab,
            "IronPickaxe" => ironPickaxePrefab,
            "DiamondPickaxe" => diamondPickaxePrefab,
            "AlloyPickaxe" => alloyPickaxePrefab, //미스릴곡괭이
            _ => null
        };
    }
}
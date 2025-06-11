using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldWeaponActivator : MonoBehaviour
{
    [Header("Holder 하위 무기 프리팹들 (모두 포함)")]
    public List<GameObject> allWeaponModels;

    public void ActivateWeaponModel(string weaponName)
    {
        Debug.Log("[HeldWeaponActivator] 무기 모델 활성화 시도: " + weaponName);
        bool matched = false;

        foreach (var model in allWeaponModels)
        {
            bool isMatch = model.name == weaponName;
            model.SetActive(isMatch);

            if (isMatch)
            {
                Debug.Log("[HeldWeaponActivator] 무기 모델 활성화됨: " + model.name);
                matched = true;
            }
        }

        if (!matched)
        {
            Debug.LogWarning("[HeldWeaponActivator] 해당 이름의 무기 모델을 찾을 수 없음: " + weaponName);
        }
    }
}
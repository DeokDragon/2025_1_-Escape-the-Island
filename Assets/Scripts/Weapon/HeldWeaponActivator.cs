using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldWeaponActivator : MonoBehaviour
{
    [Header("Holder ���� ���� �����յ� (��� ����)")]
    public List<GameObject> allWeaponModels;

    public void ActivateWeaponModel(string weaponName)
    {
        Debug.Log("[HeldWeaponActivator] ���� �� Ȱ��ȭ �õ�: " + weaponName);
        bool matched = false;

        foreach (var model in allWeaponModels)
        {
            bool isMatch = model.name == weaponName;
            model.SetActive(isMatch);

            if (isMatch)
            {
                Debug.Log("[HeldWeaponActivator] ���� �� Ȱ��ȭ��: " + model.name);
                matched = true;
            }
        }

        if (!matched)
        {
            Debug.LogWarning("[HeldWeaponActivator] �ش� �̸��� ���� ���� ã�� �� ����: " + weaponName);
        }
    }
}
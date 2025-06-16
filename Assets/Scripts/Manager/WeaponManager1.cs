using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager1 : MonoBehaviour
{
    public static bool isChangeWeapon = false;
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    [SerializeField] private string currentWeaponType;

    [SerializeField] private float changeWeaponDelayTime;
    [SerializeField] private float changeWeaponEndDelayTime;

    [SerializeField] private CloseWeapon[] hands;
    [SerializeField] private CloseWeapon[] axes;
    [SerializeField] private CloseWeapon[] pickaxes;

    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();

    [SerializeField] private HandController theHandController;
    [SerializeField] private AxeController theAxeController;
    [SerializeField] private PickaxeController thePickaxeController;
    [SerializeField] private HeldWeaponActivator heldWeaponActivator;

    void Start()
    {
        for (int i = 0; i < hands.Length; i++)
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        for (int i = 0; i < axes.Length; i++)
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        for (int i = 0; i < pickaxes.Length; i++)
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        if (isChangeWeapon) yield break;
        isChangeWeapon = true;

        if (currentWeaponAnim != null)
            currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        isChangeWeapon = false;
    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "HAND":
                if (theHandController != null) theHandController.isActivate = false;
                break;
            case "AXE":
                if (theAxeController != null) theAxeController.isActivate = false;
                break;
            case "PICKAXE":
                if (thePickaxeController != null) thePickaxeController.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if (string.IsNullOrEmpty(_name)) return;

        heldWeaponActivator.ActivateWeaponModel(_name);

        currentWeaponType = _type; // 타입을 먼저 설정

        if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
        else if (_type == "PICKAXE")
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
    }

    public void EquipWeaponByName(string weaponName)
    {
        if (isChangeWeapon) return;

        string weaponType = "";
        if (axeDictionary.ContainsKey(weaponName)) weaponType = "AXE";
        else if (pickaxeDictionary.ContainsKey(weaponName)) weaponType = "PICKAXE";
        else if (handDictionary.ContainsKey(weaponName)) weaponType = "HAND";

        if (!string.IsNullOrEmpty(weaponType))
        {
            WeaponChange(weaponType, weaponName);
        }
    }
}
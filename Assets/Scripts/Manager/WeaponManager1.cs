using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManager1 : MonoBehaviour
{
    public static bool isChangeWeapon = false;

    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    [SerializeField]
    private string currentWeaponType;

    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();

    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;
    [SerializeField]
    private PickaxeController thePickaxeController;

    // ✅ 추가: 인스펙터에서 연결할 HeldWeaponActivator
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
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        // ✅ 무기 프리팹 표시 (인스펙터에서 할당된 경우만)
        heldWeaponActivator?.ActivateWeaponModel(_name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "HAND":
                HandController.isActivate = false;
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        Debug.Log("[WeaponChange] 넘겨받은 무기 이름: " + _name);

        if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
        else if (_type == "PICKAXE")
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
        heldWeaponActivator.ActivateWeaponModel(_name);
    }

    public IEnumerator WeaponInCoroutine()
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        currentWeapon.gameObject.SetActive(false);
    }

    public void WeaponOut()
    {
        isChangeWeapon = false;
        currentWeapon.gameObject.SetActive(true);
    }

    public void ChangeWeaponTo(string weaponType)
    {
        StartCoroutine(ChangeWeaponCoroutine(weaponType, "BareHand"));
    }

    public string GetEquippedWeaponName()
    {
        return currentWeaponType;
    }

    public void EquipWeaponByName(string weaponName)
    {
        Debug.Log("[WeaponManager1] EquipWeaponByName 호출됨: " + weaponName);

        // ✅ 여기선 여전히 동적으로 찾아도 무방
        HeldWeaponActivator activator = FindObjectOfType<HeldWeaponActivator>();

        if (handDictionary.ContainsKey(weaponName))
        {
            Debug.Log("[WeaponManager1] HAND 장착 시도");
            theHandController.CloseWeaponChange(handDictionary[weaponName]);
            activator?.ActivateWeaponModel(weaponName);
            currentWeaponType = "HAND";
        }
        else if (axeDictionary.ContainsKey(weaponName))
        {
            Debug.Log("[WeaponManager1] AXE 장착 시도");
            theAxeController.CloseWeaponChange(axeDictionary[weaponName]);
            activator?.ActivateWeaponModel(weaponName);
            currentWeaponType = "AXE";
        }
        else if (pickaxeDictionary.ContainsKey(weaponName))
        {
            Debug.Log("[WeaponManager1] PICKAXE 장착 시도");
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[weaponName]);
            activator?.ActivateWeaponModel(weaponName);
            currentWeaponType = "PICKAXE";
        }
        else
        {
            Debug.LogWarning($"[WeaponManager1] 무기 이름 '{weaponName}'을 찾을 수 없습니다.");
        }
    }
}
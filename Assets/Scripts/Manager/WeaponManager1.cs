using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManager1 : MonoBehaviour
{
    // 무기 중복 교체 실행 방지.
    // 무기 중복 교체 실행 방지.
    public static bool isChangeWeapon = false;

    // 현재 무기와 현재 무기의 애니메이션.
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    // 현재 무기의 타입.
    [SerializeField]
    private string currentWeaponType;


    // 무기 교체 딜레이, 무기 교체가 완전히 끝난 시점.
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;


    //무기 종류
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    // 관리 차원에서 쉽게 무기 접근이 가능하도록 만듦.

    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();


    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;
    [SerializeField]
    private PickaxeController thePickaxeController;

    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < hands.Length; i++)
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        for (int i = 0; i < axes.Length; i++)
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        for (int i = 0; i < pickaxes.Length; i++)
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
    }


    // 무기 교체 코루틴.
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    // 무기 취소 함수.
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

    // 무기 교체 함수.
    private void WeaponChange(string _type, string _name)
    {

        if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
        else if (_type == "PICKAXE")
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
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
        StartCoroutine(ChangeWeaponCoroutine(weaponType, "BareHand")); // 이름은 기본 무기로 대체
    }


    public string GetEquippedWeaponName()
    {
        return currentWeaponType;
    }

    // 🔹 저장 불러온 무기 이름으로 장착하는 함수
    public void EquipWeaponByName(string weaponName)
    {
        if (handDictionary.ContainsKey(weaponName))
            theHandController.CloseWeaponChange(handDictionary[weaponName]);
        else if (axeDictionary.ContainsKey(weaponName))
            theAxeController.CloseWeaponChange(axeDictionary[weaponName]);
        else if (pickaxeDictionary.ContainsKey(weaponName))
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[weaponName]);
        else
            Debug.LogWarning($"무기 이름 '{weaponName}'을 찾을 수 없습니다.");
    }


}

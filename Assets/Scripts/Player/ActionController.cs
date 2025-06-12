using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;// 습득 가능 최대 거리

    private bool pickupActivated = false;


    private RaycastHit hitInfo; //충돌체 정보 저장

    //아이템 레이어에만 반응하도록 설정
    [SerializeField]
    private LayerMask layerMask;

    //필요 컴퍼넌트
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    

    // Update is called once per frame
    void Update()
    {
        CheckItem();
        TryAction();    
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyBindingManager.GetKey("Interact", KeyCode.E)))
        {
            CheckItem();
            CanPickUp();
        }

    }
    private void CanPickUp()
    {
        if (!pickupActivated)
        {
            return;
        }

        if (hitInfo.transform == null)
        {
            return;
        }

        var pickupComponent = hitInfo.transform.GetComponent<ItemPickUp>();
        if (pickupComponent == null)
        {
            return;
        }

        if (pickupComponent.item == null)
        {
            return;
        }

        theInventory.AcquireItem(pickupComponent.item);

        WeaponManager1 weaponManager = FindObjectOfType<WeaponManager1>();
        if (weaponManager != null)
        {
            weaponManager.EquipWeaponByName(pickupComponent.item.itemName);
        }

        Destroy(hitInfo.transform.gameObject);
        InfoDisappear();
    }



    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {

            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            else
            {
                Debug.Log("태그 불일치: " + hitInfo.transform.tag);
            }
        }
        else
        {
            InfoDisappear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
   
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}

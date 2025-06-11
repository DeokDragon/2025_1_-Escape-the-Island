using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;// ���� ���� �ִ� �Ÿ�

    private bool pickupActivated = false;


    private RaycastHit hitInfo; //�浹ü ���� ����

    //������ ���̾�� �����ϵ��� ����
    [SerializeField]
    private LayerMask layerMask;

    //�ʿ� ���۳�Ʈ
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
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }    
    }
    private void CanPickUp()
    {
        if (!pickupActivated)
        {
            Debug.Log("[CanPickUp] pickupActivated�� false�Դϴ�.");
            return;
        }

        if (hitInfo.transform == null)
        {
            Debug.Log("[CanPickUp] hitInfo.transform�� null�Դϴ�.");
            return;
        }

        var pickupComponent = hitInfo.transform.GetComponent<ItemPickUp>();
        if (pickupComponent == null)
        {
            Debug.Log("[CanPickUp] ItemPickUp ������Ʈ�� �����ϴ�. �浹�� ������Ʈ: " + hitInfo.transform.name);
            return;
        }

        if (pickupComponent.item == null)
        {
            Debug.Log("[CanPickUp] item�� null�Դϴ�. ItemPickUp ������Ʈ: " + hitInfo.transform.name);
            return;
        }

        Debug.Log("[ActionController] ȹ�� ������ �̸�: " + pickupComponent.item.itemName);

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
            Debug.Log("Ray �浹 ����: " + hitInfo.transform.name);

            if (hitInfo.transform.tag == "Item")
            {
                Debug.Log("�±� ��ġ: Item");
                ItemInfoAppear();
            }
            else
            {
                Debug.Log("�±� ����ġ: " + hitInfo.transform.tag);
            }
        }
        else
        {
            Debug.Log("Ray �浹 ����");
            InfoDisappear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ�� " + "<color=yellow>" + "(E)" + "</color>";
   
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}

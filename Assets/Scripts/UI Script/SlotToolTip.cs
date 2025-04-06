using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField] private GameObject go_Base;
    [SerializeField] private Text txt_ItemName;
    [SerializeField] private Text txt_ItemDesc;
    [SerializeField] private Text txt_ItemHowtoUsed;

    private RectTransform rt;

    private void Awake()
    {
        rt = go_Base.GetComponent<RectTransform>();
        HideToolTip();
    }

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);

        // Canvas ���� ���� ��ġ
        Vector3 offset = new Vector3(rt.rect.width * 0.5f, -rt.rect.height, 0f);
        go_Base.transform.position = _pos + offset;

        txt_ItemName.text = _item.itemName;
        txt_ItemDesc.text = _item.itemDesc;

        switch (_item.itemType)
        {
            case Item.ItemType.Equipment:
                txt_ItemHowtoUsed.text = "��Ŭ�� - ����";
                break;
            case Item.ItemType.Used:
                txt_ItemHowtoUsed.text = "��Ŭ�� - �Ա�";
                break;
            default:
                txt_ItemHowtoUsed.text = "";
                break;
        }
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);

        // �ؽ�Ʈ �ʱ�ȭ (�ܻ� ����)
        txt_ItemName.text = "";
        txt_ItemDesc.text = "";
        txt_ItemHowtoUsed.text = "";
    }
}

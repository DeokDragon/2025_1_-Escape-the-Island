using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffectDataBase : MonoBehaviour
{
     
        //�ʿ��� ������Ʈ      
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;





    //slot tool tip ¡�˴ٸ�
    public void ShowToolTop(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item, _pos);
    }


    // QuickSlotController ¡�˴ٸ�.
    public void IsActivatedQuickSlot(int _num)
    {
        theQuickSlotController.IsActivatedQuickSlot(_num);
    }

    // SlotToolTip ¡�˴ٸ�.
    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }
}

        



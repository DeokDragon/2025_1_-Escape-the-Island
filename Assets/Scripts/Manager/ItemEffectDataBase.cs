using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffectDataBase : MonoBehaviour
{
     
        //ÇÊ¿äÇÑ ÄÄÆ÷³ÍÆ®      
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;





    //slot tool tip Â¡°Ë´Ù¸®
    public void ShowToolTop(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item, _pos);
    }


    // QuickSlotController Â¡°Ë´Ù¸®.
    public void IsActivatedQuickSlot(int _num)
    {
        theQuickSlotController.IsActivatedQuickSlot(_num);
    }

    // SlotToolTip Â¡°Ë´Ù¸®.
    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }
}

        



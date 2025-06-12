using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 조합법 스크립트가 상속받아야 할 추상 클래스
// MonoBehaviour를 상속받아 게임 오브젝트에 붙일 수 있도록 함
public abstract class BaseRecipe : MonoBehaviour
{
    public abstract bool CheckRecipe(Slot toolSlot, Slot[] materialSlots);

    public abstract void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot);

    public abstract Item GetPreviewResult();
}
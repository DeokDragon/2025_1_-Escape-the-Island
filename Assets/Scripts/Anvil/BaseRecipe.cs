using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 조합법 스크립트가 상속받아야 할 추상 클래스
// MonoBehaviour를 상속받아 게임 오브젝트에 붙일 수 있도록 함
public abstract class BaseRecipe : MonoBehaviour
{
    // [조합 조건 확인]
    // 이 레시피가 현재 슬롯 상태에서 실행될 수 있는지 확인합니다.
    // 각 조합법 스크립트에서 이 부분을 자신에 맞게 구현해야 합니다.
    public abstract bool CheckRecipe(Slot toolSlot, Slot[] materialSlots);

    // [조합 실행]
    // 실제 조합을 실행하고, 재료를 소모하며, 결과물을 생성합니다.
    public abstract void Craft(Slot toolSlot, Slot[] materialSlots, Slot resultSlot);

    // [미리보기 제공]
    // 조합 결과물이 무엇인지 미리보기용으로 반환합니다.
    public abstract Item GetPreviewResult();
}
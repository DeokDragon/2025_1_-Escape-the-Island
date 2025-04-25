using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player"; // 타겟 태그 (기본값: Player)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Burn burn = other.GetComponent<Burn>();
            if (burn != null)
            {
                burn.StartBurning();
                Debug.Log(" 불꽃 트리거 작동됨 → StartBurning() 호출됨!");
            }
            else
            {
                Debug.LogWarning(" Burn 스크립트가 Player에 없음!");
            }
        }
    }
}

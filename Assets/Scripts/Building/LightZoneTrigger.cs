using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightZoneTrigger : MonoBehaviour
{
  
    [SerializeField] private float innerRadius = 1.5f;  // 완전히 밝은 거리
    [SerializeField] private float outerRadius = 5f;    // 전혀 밝지 않은 거리
    [SerializeField] private Transform torchCenter;     // 횃불 중심 위치

    private bool isPlayerInside = false;
    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!CaveStateManager.Instance.IsPlayerInsideCave) return; //동굴 밖이면 무시

        player = other.transform;
        isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = false;
        ResetFog();  // 플레이어가 완전히 나가면 다시 동굴 안개로 복귀
    }

    private void Update()
    {
        // ① 동굴 안이 아니면 아예 아무 효과도 주지 않음
        if (!isPlayerInside || player == null || !CaveStateManager.Instance.IsPlayerInsideCave)
            return;

        float distance = Vector3.Distance(player.position, torchCenter.position);

        // 0 ~ 1 사이 보간 비율 구하기
        float t = Mathf.InverseLerp(outerRadius, innerRadius, distance);
        t = Mathf.Clamp01(t); // 안정성 보장

        // 안개와 밝기 보간 (지금은 어둠 유지지만 구조 유지)
        RenderSettings.ambientLight = Color.Lerp(Color.black, Color.black, t);
        RenderSettings.fogColor = Color.Lerp(Color.black, Color.black, t);
        RenderSettings.fogStartDistance = Mathf.Lerp(0.8f, 4f, t);
        RenderSettings.fogEndDistance = Mathf.Lerp(3f, 20f, t);
    }

    private void ResetFog()
    {
        RenderSettings.ambientLight = Color.black;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogStartDistance = 0.8f;
        RenderSettings.fogEndDistance = 3f;
      
    }
}

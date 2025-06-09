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
        if (!isPlayerInside || player == null) return;

        float distance = Vector3.Distance(player.position, torchCenter.position);
        float t = Mathf.InverseLerp(outerRadius, innerRadius, distance);  // 거리에 따라 0~1로 보간

        // 안개/밝기 보간
        RenderSettings.ambientLight = Color.Lerp(Color.black, Color.gray, t);
        RenderSettings.fogColor = Color.Lerp(Color.black, Color.gray, t);
        RenderSettings.fogStartDistance = Mathf.Lerp(2f, 6f, t);
        RenderSettings.fogEndDistance = Mathf.Lerp(13f, 30f, t);
        RenderSettings.fogDensity = Mathf.Lerp(0.08f, 0.005f, t);
    }

    private void ResetFog()
    {
        RenderSettings.ambientLight = Color.black;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogStartDistance = 2f;
        RenderSettings.fogEndDistance = 13f;
        RenderSettings.fogDensity = 0.08f;
    }
}

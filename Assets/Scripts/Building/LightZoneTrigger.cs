using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightZoneTrigger : MonoBehaviour
{
    [SerializeField] private float innerRadius = 1.5f;
    [SerializeField] private float outerRadius = 5f;
    [SerializeField] private Transform torchCenter;

    private bool isPlayerInside = false;
    private Transform player;
    private bool isInCave = false;

    private void Start()
    {
        // 주변에 "Cave" 태그를 가진 오브젝트가 있는지 확인
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Cave"))
            {
                isInCave = true;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!CaveStateManager.Instance.IsPlayerInsideCave) return;
        if (!isInCave) return;

        player = other.transform;
        isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = false;

        if (isInCave)
            ResetFog();
    }

    private void Update()
    {
        if (!isPlayerInside || player == null) return;
        if (!CaveStateManager.Instance.IsPlayerInsideCave) return;
        if (!isInCave) return;

        float distance = Vector3.Distance(player.position, torchCenter.position);
        float t = Mathf.InverseLerp(outerRadius, innerRadius, distance);
        t = Mathf.Clamp01(t);

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

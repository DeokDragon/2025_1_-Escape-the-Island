using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntranceTrigger : MonoBehaviour
{
    public Transform caveForwardDirection;  // 동굴이 바라보는 방향 (앞쪽)
    private bool isInsideCave = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 toPlayer = other.transform.position - transform.position;
        float dot = Vector3.Dot(toPlayer.normalized, caveForwardDirection.forward);

        // caveForwardDirection.forward 기준으로 -값이면 cave 내부로 들어간 것
        if (!isInsideCave && dot < 0f)
        {
            EnterCave();
            isInsideCave = true;
        }
        else if (isInsideCave && dot > 0f)
        {
            ExitCave();
            isInsideCave = false;
        }
    }

    private void EnterCave()
    {

        CaveStateManager.Instance.SetCaveState(true);

        RenderSettings.ambientLight = Color.black;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 2f;
        RenderSettings.fogEndDistance = 10f;  // 가까운 거리에서 포그 시작
        RenderSettings.fogDensity = 0.1f;    // Linear 모드에서는 크게 영향 없음

        Debug.Log("Entered cave - dark on");
    }

    private void ExitCave()
    {
        CaveStateManager.Instance.SetCaveState(false);

        RenderSettings.ambientLight = Color.white;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 50f;
        RenderSettings.fogEndDistance = 300f;
        RenderSettings.fogDensity = 0.001f;

        Debug.Log("Exited cave - bright on");
    }
}

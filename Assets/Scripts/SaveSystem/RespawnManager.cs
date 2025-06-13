using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    public float respawnDelay = 3f;
    public FadeController fadeController;

    private bool isRespawning = false;
    private bool wasInCave = false;

    public void Respawn()
    {
        if (!isRespawning)
        {
            // 부활 시작 전에 동굴 상태 저장
            wasInCave = CaveStateManager.Instance != null && CaveStateManager.Instance.IsPlayerInsideCave;
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (fadeController != null)
            fadeController.FadeOut();

        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(respawnDelay);

        // 🔥 화상 끄기
        Burn burn = player.GetComponent<Burn>();
        if (burn != null)
            burn.Off();

        // 위치 이동
        player.transform.position = respawnPoint.position;

        // 카메라 리셋
        player.GetComponent<PlayerController>().ResetCameraRotation();

        // ✅ ✅ ✅ 핵심 환경/밝기 초기화
        ResetEnvironment();

        // 체력 회복
        StatusController status = player.GetComponent<StatusController>();
        if (status != null)
        {
            status.SetStatus(status.hp, status.sp, status.hungry, status.thirsty);
            status.ResetHungryThirstyTimer();
        }

        player.GetComponent<PlayerController>().enabled = true;
        player.SetActive(true);

        if (fadeController != null)
            fadeController.FadeIn();

        isRespawning = false;
    }

    private void ResetEnvironment()
    {
        // ✅ 안개 / 포그 / 시야 리셋
        RenderSettings.ambientLight = Color.white;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 50f;
        RenderSettings.fogEndDistance = 300f;
        RenderSettings.fogDensity = 0.001f;

        // ✅ 동굴 상태도 야외로 강제 설정
        if (CaveStateManager.Instance != null)
            CaveStateManager.Instance.SetCaveState(false);

        // ✅ DayNight 외부 밝기 보정 (혹시 낮밤 시스템 쓸 경우)
        DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
        if (timeSystem != null)
            timeSystem.ForceOutsideReset();

        // ✅ 🔥 여기 가장 중요한 핵심: 횃불 라이트 초기화
        DisableAllTorches();
    }

    private void DisableAllTorches()
    {
        // 🔥 이름으로 탐색 (이름에 "Torch"가 포함된 오브젝트 비활성화)
        var allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("Torch"))
            {
                obj.SetActive(false);
            }
        }
    }

}

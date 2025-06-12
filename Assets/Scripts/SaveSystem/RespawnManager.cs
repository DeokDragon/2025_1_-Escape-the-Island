using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    public float respawnDelay = 3f;

    public FadeController fadeController;

    private bool isRespawning = false; 

    public void Respawn()
    {
        if (!isRespawning)
        {
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

        Burn burn = player.GetComponent<Burn>();
        if (burn != null)
            burn.Off();

        // 위치 이동
        player.transform.position = respawnPoint.position;

        // 동굴 상태를 바깥으로 설정
        CaveStateManager.Instance.SetCaveState(false);

        // 환경 초기화 (ExitCave와 같은 효과)
        RenderSettings.ambientLight = Color.white;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 50f;
        RenderSettings.fogEndDistance = 300f;
        RenderSettings.fogDensity = 0.001f;

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
}

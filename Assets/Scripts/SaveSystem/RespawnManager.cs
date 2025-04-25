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

        // 플레이어 조작 잠깐 막기
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(respawnDelay);

        // 위치 이동
        player.transform.position = respawnPoint.position;


        // 체력 회복
        StatusController status = player.GetComponent<StatusController>();
        if (status != null)
        {
            // 최대값
            status.SetMaxStatus(100, 100, 100, 100); //  새로 추가할 함수

            status.SetStatus(100, 100, 100, 100);    // 현재값 설정
            status.ResetHungryThirstyTimer();        // (기존 유지)
        }

        // 조작 복구
        player.GetComponent<PlayerController>().enabled = true;
        player.SetActive(true);

        if (fadeController != null)
            fadeController.FadeIn();

        

        isRespawning = false; // 🔥 추가
    }
}

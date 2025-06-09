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

        Burn burn = player.GetComponent<Burn>();
        if (burn != null)
            burn.Off();

        // 위치 이동
        player.transform.position = respawnPoint.position;


        // 체력 회복
        StatusController status = player.GetComponent<StatusController>();
        if (status != null)
        {
           

            // 현재값만 최대값으로 채우기
            status.SetStatus(status.hp, status.sp, status.hungry, status.thirsty);

            status.ResetHungryThirstyTimer(); // (기존 유지)
        }


        // 조작 복구
        player.GetComponent<PlayerController>().enabled = true;
        player.SetActive(true);

        if (fadeController != null)
            fadeController.FadeIn();

        

        isRespawning = false; //  추가
    }
}

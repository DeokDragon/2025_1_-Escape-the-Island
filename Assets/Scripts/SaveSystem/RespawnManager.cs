using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    public float respawnDelay = 3f;

    public FadeController fadeController;  //  이거 클래스 안에 있어야 함!

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        player.SetActive(false);

        if (fadeController != null)
            fadeController.FadeOut();

        yield return new WaitForSeconds(respawnDelay);

        player.transform.position = respawnPoint.position;

        StatusController status = player.GetComponent<StatusController>();
        status.SetStatus(100, 100, 100, 100);
        status.Invoke("GaugeUpdate", 0f);

        player.SetActive(true);

        if (fadeController != null)
            fadeController.FadeIn();

        
    }
}

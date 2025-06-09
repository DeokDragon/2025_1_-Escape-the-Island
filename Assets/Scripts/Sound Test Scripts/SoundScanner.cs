using UnityEngine;

public class SoundScanner : MonoBehaviour
{
    void Start()
    {
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>(true); // 비활성도 포함

        foreach (AudioSource source in allAudio)
        {
            if (source.clip != null)
            {
                Debug.Log($" 발견됨: GameObject = {source.gameObject.name}, Clip = {source.clip.name}, PlayOnAwake = {source.playOnAwake}, isPlaying = {source.isPlaying}");
            }
        }
    }
}

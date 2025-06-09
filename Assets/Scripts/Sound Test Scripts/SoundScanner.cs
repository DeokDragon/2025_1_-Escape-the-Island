using UnityEngine;

public class SoundScanner : MonoBehaviour
{
    void Start()
    {
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>(true); // ��Ȱ���� ����

        foreach (AudioSource source in allAudio)
        {
            if (source.clip != null)
            {
                Debug.Log($" �߰ߵ�: GameObject = {source.gameObject.name}, Clip = {source.clip.name}, PlayOnAwake = {source.playOnAwake}, isPlaying = {source.isPlaying}");
            }
        }
    }
}

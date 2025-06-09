using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;

    private string[] playSoundName;

    public Sound[] effectsSounds;
    public Sound[] bgmSounds;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];

        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);
    }

    public void PlaySE(string _name)
    {

        Debug.Log($"🔊 사운드 재생 요청됨: {name}");
        for (int i = 0; i < effectsSounds.Length; i++)
        {
            if (_name == effectsSounds[i].name)
            {
                AudioClip clip = effectsSounds[i].clip;

                // 1. 비어 있는 AudioSource가 있으면 사용
                foreach (AudioSource source in audioSourceEffects)
                {
                    if (!source.isPlaying)
                    {
                        source.clip = clip;
                        source.Play();
                        return;
                    }
                }

                // 2. 전부 재생 중이면 OneShot으로 재생 (겹침 허용)
                audioSourceEffects[0].PlayOneShot(clip);
                return;
            }
        }

        Debug.LogWarning($"❌ '{_name}' 사운드가 SoundManager에 등록되지 않았습니다.");
    }


    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }

        Debug.LogWarning($"❌ '{_name}' 사운드는 현재 재생 중이지 않습니다.");
    }

    public void StopAllSE()
    {
        foreach (var source in audioSourceEffects)
        {
            source.Stop();
        }
    }

    public void SetBGMVolume(float volume)
    {
        foreach (AudioSource bgm in audioSourceBGM)
        {
            bgm.volume = volume;
        }
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        foreach (AudioSource sfx in audioSourceEffects)
        {
            sfx.volume = volume;
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void PlayBGM(string name)
    {
        foreach (Sound bgm in bgmSounds)
        {
            if (bgm.name == name)
            {
                foreach (AudioSource source in audioSourceBGM)
                {
                    source.clip = bgm.clip;
                    source.volume = PlayerPrefs.GetFloat("BGMVolume", 1f);
                    source.Play();
                }
                return;
            }
        }

        Debug.LogWarning($"❌ '{name}' 이름의 BGM을 찾을 수 없습니다.");
    }
}
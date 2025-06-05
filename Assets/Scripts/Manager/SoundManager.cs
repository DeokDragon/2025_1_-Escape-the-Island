using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name; //곡의 이름
    public AudioClip clip; // 곡
}
public class SoundManager : MonoBehaviour
{



    static public SoundManager instance;

    //싱글턴, singleton,

    #region singleton
    void Awake()  //생성 최초

    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);


    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;

    public string[] playSoundName;

    public Sound[] effectsSounds;
    public Sound[] bgmSounds;

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));

        // 슬라이더 저장값 불러와서 초기 볼륨 설정
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);

        foreach (AudioSource bgm in audioSourceBGM)
        {
            bgm.volume = PlayerPrefs.GetFloat("BGMVolume", 1f); // 초기화시 강제 적용
        }

    }


    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectsSounds.Length; i++)
        {
            if(_name == effectsSounds[i].name)
            {
                for(int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectsSounds[i].name;
                        audioSourceEffects[j].clip = effectsSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
       
    }

    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }

    }
    
    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다");
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
                for (int i = 0; i < audioSourceBGM.Length; i++)
                {
                    audioSourceBGM[i].clip = bgm.clip;
                    audioSourceBGM[i].volume = PlayerPrefs.GetFloat("BGMVolume", 1f); // 슬라이더값 반영
                    audioSourceBGM[i].Play();
                }
                return;
            }
        }
        Debug.LogWarning($"❌ '{name}' 이름의 BGM을 찾을 수 없습니다.");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name; //���� �̸�
    public AudioClip clip; // ��
}
public class SoundManager : MonoBehaviour
{



    static public SoundManager instance;

    //�̱���, singleton,

    #region singleton
    void Awake()  //���� ����

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
                Debug.Log("��� ���� AudioSource�� ������Դϴ�");
                return;
            }
        }
        Debug.Log(_name + "���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
       
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
        Debug.Log("��� ����" + _name + "���尡 �����ϴ�");
    }

}

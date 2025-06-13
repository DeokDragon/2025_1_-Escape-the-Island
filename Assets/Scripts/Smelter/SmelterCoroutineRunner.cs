using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterCoroutineRunner : MonoBehaviour
{
    public static SmelterCoroutineRunner instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지하고 싶을 때
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }
}

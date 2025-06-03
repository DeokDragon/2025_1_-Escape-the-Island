using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterCoroutineRunner : MonoBehaviour
{
    public static SmelterCoroutineRunner instance;

    void Awake()
    {
        // 인스턴스가 비어있다면 이 오브젝트를 할당합니다
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

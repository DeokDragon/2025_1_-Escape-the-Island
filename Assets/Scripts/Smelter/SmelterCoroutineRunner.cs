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
            DontDestroyOnLoad(gameObject); // �� ��ȯ���� �����ϰ� ���� ��
        }
        else if (instance != this)
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }
}

using System.Collections;
using UnityEngine;

public class CaveRandomizer : MonoBehaviour
{
    public GameObject[] caveSpawns; // ���� ������Ʈ 3�� ����� �迭

    void Start()
    {
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);

        if (isContinue == 0)
        {
            // �� �����̸� �������� �ϳ��� Ȱ��ȭ
            int selectedIndex = Random.Range(0, caveSpawns.Length);

            for (int i = 0; i < caveSpawns.Length; i++)
            {
                caveSpawns[i].SetActive(i == selectedIndex);
            }

            
        }
    }
}

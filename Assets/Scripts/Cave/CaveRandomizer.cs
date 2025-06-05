using System.Collections;
using UnityEngine;

public class CaveRandomizer : MonoBehaviour
{
    public GameObject[] caveSpawns; // 동굴 오브젝트 3개 등록할 배열

    void Start()
    {
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);

        if (isContinue == 0)
        {
            // 새 게임이면 랜덤으로 하나만 활성화
            int selectedIndex = Random.Range(0, caveSpawns.Length);

            for (int i = 0; i < caveSpawns.Length; i++)
            {
                caveSpawns[i].SetActive(i == selectedIndex);
            }

            
        }
    }
}

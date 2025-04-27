using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    private float destroyTime; // 파편 삭제 시간

    [SerializeField]
    private SphereCollider col; // 구체 


    //필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock;//일반 바위
    [SerializeField]
    private GameObject go_debris;//깨진 바위
    [SerializeField]
    private GameObject go_effect_prefabs;
    [SerializeField]
    private GameObject go_rock_item_prefab;

    //돌맹이 등장 개수
    [SerializeField]
    private int count;

    //필요한 사운드 이름
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    //돌 오브젝트 제거
    public float objectdestoryTime;
    public GameObject objectdestory;

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);
         
       var clone =  Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);



        hp--;
        if (hp <= 0)
            Destruction();
    }
   
    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;

        for( int i = 0; i <= count; i++ )
        {
            Instantiate(go_rock_item_prefab, go_rock.transform.position, Quaternion.identity);
        }

        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);

        //일정 시간 뒤 제거
        //Invoke("objectdestorys", objectdestoryTime);
        Destroy(gameObject);
    }

    //돌 오브젝트 제거 함수
    /*void objectdestorys()
    {
        Destroy(objectdestory);
        Debug.Log($"{objectdestory}를 제거 했습니다.");
    }*/

}

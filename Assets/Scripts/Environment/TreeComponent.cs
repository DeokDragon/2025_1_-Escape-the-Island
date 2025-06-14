using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeComponent : MonoBehaviour
{

    // 깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;

    // 통나무.
    [SerializeField]
    private GameObject go_Log_Prefabs;

    //나뭇가지
    [SerializeField]
    private GameObject go_Twig_Prefab;

    // 쓰러질 때의 랜덤으로 가해질 힘의 세기
    [SerializeField]
    private float force;


    // 자식 트리.
    [SerializeField]
    private GameObject go_ChildTree;


    // 부모 트리 파괴되면, 캡슐 콜라이더 제거.
    [SerializeField]
    private CapsuleCollider parentCol;


    // 자식 트리 쓰러질 때 필요한 컴포넌트 활성화 및 중력 활성화.
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    // 파편.
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    // 파편 제거 시간.
    [SerializeField]
    private float debrisDestroyTime;

    // 나무 제거 시간.
    [SerializeField]
    private float destroyTime;


    // 필요한 사운드
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;

    //이것들
    public float objectdestoryTime;
    public GameObject objectdestory;

    public void Chop(Vector3 _pos, float angleY)
    {
        Hit(_pos);

        AngleCalc(angleY);

        if (CheckTreePieces())
            return;

        FallDownTree();
    }

    // 적중 이펙트.
    private void Hit(Vector3 _pos)
    {
        SoundManager.instance.PlaySE(chop_sound);

        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestroyTime);
    }

    private void AngleCalc(float _angleY)
    {
        if (0 <= _angleY && _angleY <= 70)
            DestroyPiece(2);
        else if (70 <= _angleY && _angleY <= 140)
            DestroyPiece(3);
        else if (140 <= _angleY && _angleY <= 210)
            DestroyPiece(4);
        else if (210 <= _angleY && _angleY <= 280)
            DestroyPiece(0);
        else if (280 <= _angleY && _angleY <= 360)
            DestroyPiece(1);
    }

    private void DestroyPiece(int _num)
    {
        if (go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[_num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestroyTime);
            Destroy(go_treePieces[_num].gameObject);
        }
    }

    private bool CheckTreePieces()
    {
        for (int i = 0; i < go_treePieces.Length; i++)
        {
            if (go_treePieces[i].gameObject != null)
            {
                return true;
            }
        }
        return false;
    }

    private void FallDownTree()
    {
        SoundManager.instance.PlaySE(falldown_sound);
        Destroy(go_treeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force, force), 0f, Random.Range(-force, force));


        StartCoroutine(LogCoroutine());

        //이거 추가함(캐지고 일정 시간 뒤에 나무 오브젝트 제거)
        Invoke("objectdestorys", objectdestoryTime);
    }

    //나무 오브젝트 제거 함수
    void objectdestorys()
    {
        Destroy(objectdestory);
    }


    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_sound);

        // Log 생성
        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = go_ChildTree.transform.up * (3f * (i + 1));
            Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + offset, Quaternion.LookRotation(go_ChildTree.transform.up));
        }

        //Twig 생성
        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = go_ChildTree.transform.right * (1.5f * i); // 옆으로 살짝 벌리기
            Instantiate(go_Twig_Prefab, go_ChildTree.transform.position + offset + Vector3.up * 0.5f, Quaternion.identity);
        }

        Destroy(go_ChildTree.gameObject);
    }
}

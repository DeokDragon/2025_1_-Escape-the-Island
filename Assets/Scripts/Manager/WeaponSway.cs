using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //기존 위치
    private Vector3 originPos;

    //현재 위치
    private Vector3 currentPos;

    //sway 한계
    [SerializeField]
    private Vector3 limitPos;

    [SerializeField]
    private Vector3 smoothSway;

    
    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Inventory.inventoryActivated)
        {
            TrySway();
        }
        
    }
    
    private void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            Swaying();
        }
        else
            BackToOriginPos();
    }

    private void Swaying()
    {
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, -limitPos.x),
                       Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveX, smoothSway.y), -limitPos.y, -limitPos.y),
                       originPos.z);
       
        transform.localPosition = currentPos;
    }

    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}

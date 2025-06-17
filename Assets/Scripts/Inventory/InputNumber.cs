using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
   
    private bool activated = false;

    [SerializeField]
    private Text text_Preview;
    [SerializeField]
    private Text text_Input;
    [SerializeField]
    private InputField if_text;

    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private ActionController thePlayer;

    private Slot currentSlot;

    void Update()
    {
        if (activated)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OK();
            else if (Input.GetKeyDown(KeyCode.Escape))
                Cancel();
        }
    }


    //public void Call()
    //{
    //    go_Base.SetActive(true);
    //    activated = true;
    //    if_text.text = "";
    //    text_Preview.text = DragSlot.instance.dragSlot.itemCount.ToString();
    //}
    public void OpenDropUI(Slot targetSlot)
    {
        if (GameManager.isChestUIOpen)
        {
            return;
        }

        currentSlot = targetSlot;

        // ✅ dragSlot에 직접 할당
        DragSlot.instance.dragSlot = targetSlot;

        go_Base.SetActive(true);
        activated = true;
        if_text.text = "";
        text_Preview.text = targetSlot.itemCount.ToString();
    }

    public void Cancel()
    {
        activated = false;
        go_Base.SetActive(false);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OK()
    {
        Debug.Log("[버리기 UI] OK 버튼 눌림");

        DragSlot.instance.SetColor(0);

        int num;
        if (text_Input.text != "")
        {
            if (CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);
                if (num > DragSlot.instance.dragSlot.itemCount)
                    num = DragSlot.instance.dragSlot.itemCount;
            }
            else
            {
                num = 1;
            }
        }
        else
            num = int.Parse(text_Preview.text);

        StartCoroutine(DropItemCoroutine(num));
    }

    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if (currentSlot.item.itemPrefab != null)
                Instantiate(currentSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);

            currentSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }

        DragSlot.instance.dragSlot = null;
        currentSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }

    private bool CheckNumber(string _argString)
    {
        char[] _tempCharArray = _argString.ToCharArray();
        bool isNumber = true;
        for (int i = 0; i < _tempCharArray.Length; i++)
        {
            if (_tempCharArray[i] >= 48 && _tempCharArray[i] <= 57)
                continue;
            isNumber = false;
        }
        return isNumber;
    }
}


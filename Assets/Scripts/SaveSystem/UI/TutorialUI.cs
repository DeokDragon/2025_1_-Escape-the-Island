using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject tutorialPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            bool isActive = tutorialPanel.activeSelf;
            tutorialPanel.SetActive(!isActive);

            if (isActive)
            {
                // 닫힐 때 → 다시 움직임 가능
                GameManager.isTutorialOpen = false;
            }
            else
            {
                // 열릴 때 → 움직임 잠금
                GameManager.isTutorialOpen = true;
            }

            GameManager.UpdateCursorState();
        }
    }

}

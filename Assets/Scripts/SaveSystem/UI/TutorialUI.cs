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
                // ���� �� �� �ٽ� ������ ����
                GameManager.isTutorialOpen = false;
            }
            else
            {
                // ���� �� �� ������ ���
                GameManager.isTutorialOpen = true;
            }

            GameManager.UpdateCursorState();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterWell : MonoBehaviour
{
    public GameObject interactPromptUI;
    public Text interactPromptText; 
    public LayerMask playerLayer;
    public float interactDistance = 3f;
    public int thirstIncreaseAmount = 20;

    private bool isPlayerNearby = false;
    private StatusController playerStatusController;

    public string bottleItemName = "IronBottle"; 
    private Slot selectedSlot;

    //오디오 설정
    public AudioClip drinkSound;       // 마실 때 나는 소리
    private AudioSource audioSource;   // 소리 재생용 오디오소스
    void Start()
    {
        playerStatusController = FindObjectOfType<StatusController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, playerLayer))
        {
            if (hit.transform.CompareTag("WaterWell"))
            {
                isPlayerNearby = true;
                interactPromptUI.SetActive(true);

                // 현재 선택된 퀵슬롯 번호 및 슬롯 가져오기
                int selectedSlotIndex = QuickSlotController.instance?.GetSelectedSlotNumber() ?? -1;
                Slot[] quickSlots = QuickSlotController.instance?.GetQuickSlots();

                string promptText = "물 마시기";

                if (quickSlots != null && selectedSlotIndex >= 0 && selectedSlotIndex < quickSlots.Length)
                {
                    selectedSlot = quickSlots[selectedSlotIndex]; 
                    if (selectedSlot.item != null && selectedSlot.item.itemName == bottleItemName)
                    {
                        promptText = "물 채우기";
                    }
                }
                else
                {
                    selectedSlot = null; 
                }

                interactPromptText.text = promptText;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (promptText == "물 채우기")
                        FillBottle();
                    else
                        DrinkWater();
                }
            }
        }
        else
        {
            isPlayerNearby = false;
            interactPromptUI.SetActive(false);
        }
    }

    void DrinkWater()
    {
        if (playerStatusController != null)
        {
            playerStatusController.IncreaseThirsty(thirstIncreaseAmount);


            if (drinkSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(drinkSound);
            }

            // 물통에 물이 차 있을 때만 물병 비우기 및 UI 업데이트
            if (selectedSlot != null && selectedSlot.isWaterFilled)
            {
                selectedSlot.isWaterFilled = false;  
                selectedSlot.UpdateBottleUI();
            }
        }
    }

    void FillBottle()
    {
        Slot[] quickSlots = QuickSlotController.instance?.GetQuickSlots();
        int selectedSlotIndex = QuickSlotController.instance?.GetSelectedSlotNumber() ?? -1;

        if (quickSlots != null && selectedSlotIndex >= 0 && selectedSlotIndex < quickSlots.Length)
        {
            Slot selectedSlot = quickSlots[selectedSlotIndex];
            if (selectedSlot.item != null && selectedSlot.item.itemName == bottleItemName)
            {
                selectedSlot.isWaterFilled = true; 
                selectedSlot.UpdateBottleUI(); 
            }
        }
    }
}

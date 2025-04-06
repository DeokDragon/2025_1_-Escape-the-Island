using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMode : MonoBehaviour
{

    private Light flashLight;
    public GameObject flashlightOverlay;
    private bool flashLightOn = false;
    // Start is called before the first frame update
    void Start()
    {
        flashLight = GameObject.Find("FlashLight").GetComponent<Light>();
        flashLight.enabled = false;
        flashlightOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(flashLightOn == false)
            {
                flashlightOverlay.SetActive(true);
                flashLight.enabled = true;
                flashLightOn = true;
                FlashLightSwitchOff();
            }
            else if(flashLightOn == true)
            {
                flashlightOverlay.SetActive(false);
                flashLight.enabled = false;
                flashlightOverlay.GetComponent<FlashLightScript>().StopDrain();
                flashLightOn = false;
            }
        }


        if(flashLightOn == true)
        {
            FlashLightSwitchOff();
        }
    }

    private void FlashLightSwitchOff()
    {
        if(flashlightOverlay.GetComponent<FlashLightScript>().batteryPower <= 0)
        {
            flashlightOverlay.SetActive(false);
            flashLight.enabled = false;
            flashlightOverlay.GetComponent<FlashLightScript>().StopDrain();
            flashLightOn = false;
        }
    }
}

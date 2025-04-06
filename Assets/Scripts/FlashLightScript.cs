using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightScript : MonoBehaviour
{
    private Image batteryChunks;
    public float batteryPower = 1.0f;
    public float drainTime = 2;
    // Start is called before the first frame update
    void OnEnable()
    {
        batteryChunks = GameObject.Find("FLBatteryChunks").GetComponent<Image>();
        InvokeRepeating("FLBatteryDrain", drainTime, drainTime);
    }
    void Update()
    {
        batteryChunks.fillAmount = batteryPower;
    }
    private void FLBatteryDrain()
    {
        if (batteryPower > 0.0f)
            batteryPower -= 0.25f;
    }
    // Update is called once per frame
    public void StopDrain()
    {
        CancelInvoke("FLBatteryDrain");
    }
}

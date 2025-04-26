using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightScript : MonoBehaviour
{
    private Image batteryChunks;
    public float batteryPower = 1.0f;
    public float drainRate = 0.25f;
    // Start is called before the first frame update
    void OnEnable()
    {
        batteryChunks = GameObject.Find("FLBatteryChunks").GetComponent<Image>();
    }
    void Update()
    {
        if (batteryPower > 0.0f)
        {
            batteryPower -= drainRate * Time.deltaTime;
            batteryPower = Mathf.Clamp01(batteryPower);
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveStateManager : MonoBehaviour
{
    public static CaveStateManager Instance { get; private set; }

    public bool IsPlayerInsideCave { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetCaveState(bool isInside)
    {
        IsPlayerInsideCave = isInside;
    }



}

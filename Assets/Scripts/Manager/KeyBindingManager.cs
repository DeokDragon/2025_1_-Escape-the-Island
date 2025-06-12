using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyBindingManager
{
    public static KeyCode GetKey(string action, KeyCode defaultKey)
    {
        string saved = PlayerPrefs.GetString(action, defaultKey.ToString());
        if (System.Enum.TryParse(saved, out KeyCode parsed))
            return parsed;
        else
            return defaultKey;
    }
}


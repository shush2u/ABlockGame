using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticOptions : MonoBehaviour
{
    static bool countdownEnabled = true;

    public void ToggleCountdown(bool toggle)
    {
        countdownEnabled = toggle;
    }

    public bool IsCountdownEnabled()
    {
        return countdownEnabled;
    }
}

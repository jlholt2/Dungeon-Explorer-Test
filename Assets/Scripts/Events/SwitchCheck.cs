using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwitchCheck
{
    public string eventSwitchID;
    public bool detectingValue;

    public bool ConditionMet()
    {
        if (!GameManager.instance.EventVariableExists(eventSwitchID))
        {
            return false;
        }
        if (detectingValue == GameManager.instance.eventSwitches[eventSwitchID])
        {
            return true;
        }
        return false;
    }
}

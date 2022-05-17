using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MathCompare { EqualTo = 0, GreaterThan = 1, LessThan = 2, GreaterThanOrEqualTo = 3, LessThanOrEqualTo = 4 }
[System.Serializable]
public class VariableCheck
{
    public string eventVarID;
    public int detectingValue;
    public MathCompare conditionCheck;

    public bool ConditionMet()
    {
        if (!GameManager.instance.EventVariableExists(eventVarID))
        {
            return false;
        }
        switch (conditionCheck)
        {
            case MathCompare.EqualTo:
                if (detectingValue == GameManager.instance.eventVariables[eventVarID])
                {
                    return true;
                }
                break;
            case MathCompare.GreaterThan:
                if (detectingValue > GameManager.instance.eventVariables[eventVarID])
                {
                    return true;
                }
                break;
            case MathCompare.LessThan:
                if (detectingValue < GameManager.instance.eventVariables[eventVarID])
                {
                    return true;
                }
                break;
            case MathCompare.GreaterThanOrEqualTo:
                if (detectingValue >= GameManager.instance.eventVariables[eventVarID])
                {
                    return true;
                }
                break;
            case MathCompare.LessThanOrEqualTo:
                if (detectingValue <= GameManager.instance.eventVariables[eventVarID])
                {
                    return true;
                }
                break;
        }
        return false;
    }
}

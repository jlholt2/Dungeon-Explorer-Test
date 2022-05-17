using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventConditions
{
    public SwitchCheck[] switchChecks;
    public VariableCheck[] varChecks;

    public EventSequence eventSequence;
    //public EventSequence OnConditionsNotMetSequence;

    public bool ConditionsMet()
    {
        if (switchChecks.Length > 0)
        {
            foreach (SwitchCheck check in switchChecks)
            {
                if(check.eventSwitchID != "")
                {
                    if (!check.ConditionMet())
                    {
                        return false;
                    }
                }
            }
        }
        if (varChecks.Length > 0)
        {
            foreach (VariableCheck check in varChecks)
            {
                if(check.eventVarID != "")
                {
                    if (!check.ConditionMet())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public EventConditions CopyEventPage()
    {
        EventConditions copyPage = new EventConditions();

        copyPage.switchChecks = this.switchChecks;
        copyPage.varChecks = this.varChecks;
        copyPage.eventSequence = this.eventSequence.CopyEventSequence();

        return copyPage;
    }
}

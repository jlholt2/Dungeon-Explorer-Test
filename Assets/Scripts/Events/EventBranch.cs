using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventBranch
{
    [Tooltip("Determines the event's position on the floor.")]
    public Vector2 mapPos;

    public EventConditions[] eventPages; // If the event conditions are met (eventPages.ConditionsMet() == true), this event seuqnece will play. Otherwise, it moves to the next event sequence in the branch.

    // Go through each Event Sequence in eventTree, checking the EventConditions, and play the first viable event sequence, or don't play an event sequence if none are viable
    public void InitializeEvents()
    {
        if(eventPages == null)
        {
            return;
        }
        if(eventPages.Length < 1)
        {
            return;
        }
        foreach (EventConditions condition in eventPages)
        {
            if (condition.eventSequence != null)
            {
                if (condition.eventSequence.events.Length > 0)
                {
                    foreach (FloorEvent fEvent in condition.eventSequence.events)
                    {
                        fEvent.mapPos = mapPos;
                        //fEvent.eventSequence = condition.eventSequence;
                    }
                }
            }
        }
        SetSequenceParent();
    }

    public void SetSequenceParent()
    {
        if (eventPages == null)
        {
            return;
        }
        if (eventPages.Length < 1)
        {
            return;
        }
        // Cycle through the eventPages until eventPages[page].ConditionsMet() returns true
        foreach (EventConditions page in eventPages)
        {
            if (page.ConditionsMet())
            {
                //page.eventSequence.parentBranch = this;
                page.eventSequence.mapPos = this.mapPos;
            }
        }
    }
    public EventActivationRange GetActivationRange()
    {
        if (eventPages == null)
        {
            return EventActivationRange.Touch;
        }
        if (eventPages.Length < 1)
        {
            return EventActivationRange.Touch;
        }
        // Cycle through the eventPages until eventPages[page].ConditionsMet() returns true
        foreach (EventConditions page in eventPages)
        {
            if (page.ConditionsMet())
            {
                return page.eventSequence.activationRange;
            }
        }
        return EventActivationRange.Touch;
    }
    public bool GetAutoActivate()
    {
        if (eventPages == null)
        {
            return false;
        }
        if (eventPages.Length < 1)
        {
            return false;
        }
        // Cycle through the eventPages until eventPages[page].ConditionsMet() returns true
        foreach (EventConditions page in eventPages)
        {
            if (page.ConditionsMet())
            {
                return page.eventSequence.autoActivate;
            }
        }
        return false;
    }
    public void QueueSequence()
    {
        if (eventPages == null)
        {
            return;
        }
        if (eventPages.Length < 1)
        {
            return;
        }
        // Cycle through the eventPages until eventPages[page].ConditionsMet() returns true
        foreach (EventConditions page in eventPages)
        {
            if (page.ConditionsMet())
            {
                page.eventSequence.AddEventsToQueue();
            }
        }
    }

    public EventSequence GetActiveSequence()
    {
        if (eventPages == null)
        {
            return new EventSequence();
        }
        if (eventPages.Length < 1)
        {
            return new EventSequence();
        }
        // Cycle through the eventPages until eventPages[page].ConditionsMet() returns true
        foreach (EventConditions page in eventPages)
        {
            if (page.ConditionsMet())
            {
                //Debug.Log("GetActiveSequence() => page.ConditionsMet()");
                return page.eventSequence;
            }
        }
        return new EventSequence();
    }

    public EventBranch CopyEventBranch()
    {
        EventBranch copyBranch = new EventBranch();

        copyBranch.eventPages = new EventConditions[eventPages.Length];
        copyBranch.mapPos = mapPos;

        for (int page = 0; page < eventPages.Length; page++)
        {
            copyBranch.eventPages[page] = eventPages[page].CopyEventPage();
        }

        return copyBranch;
    }
}

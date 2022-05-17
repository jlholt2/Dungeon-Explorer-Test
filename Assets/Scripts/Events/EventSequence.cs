using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OverwriteWalkable { None = 0, Walkable = 1, NonWalkable = 2 } // Whether the event should overwrite the walkable status of the tile it is on. Automatically set for certain event types, like stairs.
public enum EventActivationRange { Touch = 0, StandOn = 1 } // Determines whether the event is activated by standing on the same tile or from one tile away.
public enum FacingDirection { Up = 0, Down = 1, Left = 2, Right = 3 } // For events that spawn meshes, this affects the rotation the mesh spawns in with.
[System.Serializable]
public class EventSequence
{
    //[HideInInspector] public EventBranch parentBranch;
    [HideInInspector] public Vector2 mapPos;

    public FacingDirection eventOrientation;

    [Tooltip("Whether the event should activate automatically or manually.")]
    public bool autoActivate = false;
    public ActivateDirParams activateDirParams = new ActivateDirParams();
    [System.Serializable]
    public class ActivateDirParams
    {
        public bool activateLeft = false;
        public bool activateRight = false;
        public bool activateUp = false;
        public bool activateDown = false;
    }

    [Tooltip("Whether the event should overwrite the walkable status of the tile it is on. Automatically set for certain event types, like stairs.")]
    public OverwriteWalkable overwriteWalkable;
    [Tooltip("Determines whether the event is activated by standing on the same tile or from one tile away.")]
    public EventActivationRange activationRange;

    //public EventConditions conditions; // If the event conditions are met (conditions.ConditionsMet() == true), this event seuqnece will play. Otherwise, it moves to the next event sequence in the branch.
    public FloorEvent[] events;

    public void ArrayFailsafe()
    {
        #region Event Arrays failsafe
        //if (floorEvents == null)
        //{
        //    floorEvents = new FloorEvent[1];
        //    floorEvents[0].mapPos = new Vector2(-1, -1);
        //}
        #endregion
    }

    public void AddEventsToQueue()
    {
        if(events == null)
        {
            return;
        }
        if(events.Length < 1)
        {
            return;
        }
        if (CanInteract())
        {
            //Debug.Log("Event being added to queue, can interact");
            foreach (FloorEvent fEvent in events)
            {
                GameManager.instance.NewIEvent(fEvent);
            }
        }
    }

    public bool CanInteract()
    {
        switch (GameObject.Find("OWPlayer").transform.eulerAngles.y)
        {
            case 90:
            case -270:
                return activateDirParams.activateLeft;
            case -180:
            case 180:
                return activateDirParams.activateUp;
            case -90:
            case 270:
                return activateDirParams.activateRight;
            case 0:
                return activateDirParams.activateDown;
            default:
                return false;
        }
    }

    public void SetEventInteractable(bool auto, ActivateDirParams newActivateDirParams)
    {
        SetEventInteractable(auto, newActivateDirParams.activateUp, newActivateDirParams.activateLeft, newActivateDirParams.activateDown, newActivateDirParams.activateRight);
    }
    public void SetEventInteractable(bool auto, bool up, bool left, bool down, bool right)
    {
        this.autoActivate = true;
        this.activateDirParams.activateUp = true;
        this.activateDirParams.activateLeft = true;
        this.activateDirParams.activateRight = true;
        this.activateDirParams.activateDown = true;
    }

    public EventSequence CopyEventSequence()
    {
        EventSequence copySequence = new EventSequence();

        copySequence.eventOrientation = this.eventOrientation;
        copySequence.SetEventInteractable(this.autoActivate, this.activateDirParams);
        copySequence.overwriteWalkable = this.overwriteWalkable;
        copySequence.activationRange = this.activationRange;

        copySequence.events = new FloorEvent[events.Length];

        for (int fEvent = 0; fEvent < events.Length; fEvent++)
        {
            copySequence.events[fEvent] = events[fEvent].CopyFloorEvent();
        }

        return copySequence;
    }
}

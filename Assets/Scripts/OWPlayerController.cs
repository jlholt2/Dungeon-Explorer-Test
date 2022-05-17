using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWPlayerController : MonoBehaviour
{
    LevelGenerator levelGenerator;

    public bool ignoreCollision;
    public bool eventInteraction = true;

    public bool smoothTransition = true;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    public Vector3 TargetGridPos { get { return targetGridPos; } set { targetGridPos = value; } }
    Vector3 targetGridPos;
    public Vector3 PrevTargetGridPos { get { return prevTargetGridPos; } set { prevTargetGridPos = value; } }
    Vector3 prevTargetGridPos;
    public Vector3 TargetRotation { get { return targetRotation; } set { targetRotation = value; } }
    Vector3 targetRotation;

    public EventSequence StandingEvent { get { return standingEvent; } }
    [SerializeField] EventSequence standingEvent;
    public EventSequence FrontEvent { get { return frontEvent; } }
    [SerializeField] EventSequence frontEvent;

    public bool StandingOnWalkableTile; 

    private void Start()
    {
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
    }

    private void Update()
    {
        if(GameManager.instance.owPlayer == null)
        {
            GameManager.instance.owPlayer = this;
        }
        //if (DetectEvent())
        //{
        //    // check if event auto activates, and if so, activate the event
        //    // otherwise, the player can press a button to trigger the event with currentEvent.DoTrigger();
        //    if (currentEvent.autoActivate && new Vector2(targetGridPos.x, targetGridPos.z) == currentEvent.mapPos)
        //    {
        //        currentEvent.TriggerEvent();
        //    }
        //}

        //if(frontEvent != null)
        //{
        //    Debug.Log(frontEvent.CanInteract());
        //}
    }

    private void FixedUpdate()
    {
        MovePlayer();
        StandingOnWalkableTile = (levelGenerator.walkable[(int)transform.position.x, (int)transform.position.z]);
    }

    private void MovePlayer()
    {
        if (levelGenerator.walkable[Mathf.RoundToInt(targetGridPos.x), Mathf.RoundToInt(targetGridPos.z)] || ignoreCollision)
        {
            prevTargetGridPos = targetGridPos;

            Vector3 targetPosition = targetGridPos;

            if (targetRotation.y > 270 && targetRotation.y < 361f) targetRotation.y = 0f;
            if (targetRotation.y < 0f) targetRotation.y = 270f;

            if(!smoothTransition)
            {
                transform.position = targetPosition;
                transform.rotation = Quaternion.Euler(targetRotation);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
            }
        }
        else
        {
            targetGridPos = prevTargetGridPos;
        }
    }

    public bool DetectEvent()
    {
        if (CheckForStandingEvent(new Vector2(transform.position.x,transform.position.z)))
        {
            return true;
        }
        else
        {
            if (CheckForFrontEvent(new Vector2(transform.position.x+transform.forward.x, transform.position.z + transform.forward.z)))
            {
                if (frontEvent.activationRange == EventActivationRange.Touch)
                {
                    return true;
                }
                else
                {
                    frontEvent = null;
                }
            }
            else
            {

                frontEvent = null;
            }
        }
        return false;
    }

    private bool CheckForStandingEvent(Vector2 pos)
    {
        if (!eventInteraction)
        {
            return false;
        }

        // check on the tile at pos an event, make it the standingEvent if so, then return
        if (levelGenerator.floorEvents[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)] != null)
        {
            standingEvent = levelGenerator.floorEvents[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)].GetActiveSequence();
            return true;
        }
        standingEvent = null;
        return false;
    }
    private bool CheckForFrontEvent(Vector2 pos)
    {
        if (!eventInteraction)
        {
            return false;
        }

        // check on the tile at pos an event, make it the standingEvent if so, then return
        if (levelGenerator.floorEvents[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)] != null)
        {
            //Debug.Log("Front Event Detecting!");
            frontEvent = levelGenerator.floorEvents[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)].GetActiveSequence();
            return true;
        }
        frontEvent = null;
        return false;
    }

    public void RotateLeft() { if (AtRest) targetRotation -= Vector3.up* 90f; ClearStoredEvents(); }
    public void RotateRight() { if (AtRest) targetRotation += Vector3.up* 90f; ClearStoredEvents(); }
    public void MoveForward() {
        //Debug.Log("MoveForward()");
        if (AtRest) targetGridPos += transform.forward;
        //Debug.Log("Activation Position: " + (new Vector2(targetGridPos.x, targetGridPos.z)));
        if (DetectEvent())
        {
            // check if event auto activates, and if so, activate the event
            // otherwise, the player can press a button to trigger the event with currentEvent.TriggerEvent();
            if (IgnoreEvent(frontEvent))
            {
                return;
            }
            if (frontEvent.autoActivate && frontEvent.activationRange == EventActivationRange.Touch && new Vector2(targetGridPos.x, targetGridPos.z) == frontEvent.mapPos)
            {
                frontEvent.AddEventsToQueue();
            }
        }
    }
    public void MoveBackward() { if (AtRest) targetGridPos -= transform.forward; ClearStoredEvents(); }
    public void MoveLeft() { if (AtRest) targetGridPos -= transform.right; ClearStoredEvents(); }
    public void MoveRight() { if (AtRest) targetGridPos += transform.right; ClearStoredEvents(); }

    public bool AtRest
    {
        get {
            if((Vector3.Distance(transform.position, targetGridPos) < 0.05f) &&
                (Vector3.Distance(transform.eulerAngles,targetRotation)<0.05f))
            {
                return true;
            }return false;
        }
    }

    public void ClearStoredEvents()
    {
        standingEvent = null;
        frontEvent = null;
    }

    public bool IgnoreEvent(EventSequence eventToCheck)
    {
        if(eventToCheck == null)
        {
            return true;
        }
        if(eventToCheck.mapPos.x == 0 || eventToCheck.mapPos.y == 0)
        {
            return true;
        }
        /// Change below check if needed (if conditions do not allow for ignoring event sequences when running out of pages to check)
        //if(eventToCheck.typeOfEvent == EventType.Door && GameManager.instance.EventSwitchExists(eventToCheck.eventID))
        //{
        //    if (!GameManager.instance.eventSwitches[eventToCheck.eventID])
        //    {
        //        return true;
        //    }
        //}
        return false;
    }
}

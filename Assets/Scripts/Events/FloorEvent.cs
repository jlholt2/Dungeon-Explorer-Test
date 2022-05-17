using UnityEngine;

// While making the property drawer, comment out lines 33-72, 141, 162-190, 220-224, 329-359, 364-369, 409, 413-421, 451-458, 472-479, 501 & 502

public enum EventType { Blank = 0, Move = 1, Wait = 2, Stairs = 3, Ladder = 4, Rope = 5, Warp = 6, Door = 7, DialogScene = 8, Battle = 9, Item = 10, Conditional = 11, SetSwitch = 12, SetVar = 13 } // The type of event.
[System.Serializable]
public partial class FloorEvent
{
    public bool useCommonEvent;
    public FloorEventScriptableObject commonEvent;

    public EventType typeOfEvent;
    [HideInInspector] public Vector2 mapPos;

    public string eventID { get { return GetEventID(); } }

    public string GetEventID()
    {
        return EventTypeString() + "-" + GameManager.instance.area + "-" + GameManager.instance.floor + mapPos.x + "," + mapPos.y; // replace the "event" part with a string variable that is autoset according to the type of event
    }
    private string EventTypeString()
    {
        switch (typeOfEvent)
        {
            case EventType.Door:
                return "door";
            case EventType.Battle:
                return "battle";
            default:
                return "event";
        }
    }
    #region Event Registers
    public void RegisterEventToSwitches()
    {
        RegisterEventToSwitches(false, true);
    }
    public void RegisterEventToSwitches(bool initVal)
    {
        RegisterEventToSwitches(initVal, true);
    }
    public void RegisterEventToSwitches(bool initVal, bool overrideExisting)
    {
        if (!GameManager.instance.EventSwitchExists(GetEventID()))
        {
            GameManager.instance.eventSwitches.Add(GetEventID(), initVal);
        }
        else if (overrideExisting)
        {
            GameManager.instance.eventSwitches[GetEventID()] = initVal;
        }
    }
    public void RegisterEventToVariables()
    {
        RegisterEventToVariables(0, true);
    }
    public void RegisterEventToVariables(int initVal)
    {
        RegisterEventToVariables(initVal, true);
    }
    public void RegisterEventToVariables(int initVal, bool overrideExisting)
    {
        if (!GameManager.instance.EventVariableExists(GetEventID()))
        {
            GameManager.instance.eventVariables.Add(GetEventID(), initVal);
        }
        else if (overrideExisting)
        {
            GameManager.instance.eventVariables[GetEventID()] = initVal;
        }
    }
    #endregion 
    // ^ For making the Property Drawer, this region can be commented out

    public void TriggerEvent()
    {
        DoEvent();
    }

    private void DoEvent()
    {
        if (useCommonEvent)
        {
            try
            {  
                commonEvent.fEvent.TriggerEvent();
                return;
	        }
	        catch (System.Exception)
	        {
                Debug.LogError("ERROR: FloorEvent set to useCommonEvent, but doesn't have one! Using ingrained event data instead.");
		        throw;
	        }
        }
        switch (typeOfEvent)
        {
            case EventType.Move:
                //Move();
                break;
            case EventType.Wait:
                //Wait();
                break;
            case EventType.Stairs:
                Stairs();
                break;
            case EventType.Ladder:
                Ladder();
                break;
            case EventType.Rope:
                Rope();
                break;
            case EventType.Warp:
                Warp();
                break;
            case EventType.Door:
                Door();
                break;
            case EventType.DialogScene:
                PlayDialogEvent();
                break;
            case EventType.Battle:
                StartBattle();
                break;
            case EventType.Item:
                ItemGet();
                break;
            case EventType.SetSwitch:
                //SetSwitch();
                break;
            case EventType.SetVar:
                //SetVar();
                break;
            default:
                AdvanceEventQueue();
                break;
        }
    }

    private void AdvanceEventQueue()
    {
        GameManager.instance.advanceEvent = true;
    }
}

#region Event-Specific Code
#region Stairs
/// <summary>
/// Stairs Event Details
/// The following code is used for Stairs. Some data is also used for Ladders.
/// </summary>
public partial class FloorEvent
{
    public StairParams stairParams;
    [System.Serializable]
    public class StairParams
    {
        public bool goingUp;
    }

    public void Stairs()
    {
        int tempFloorNum = GameManager.instance.floor;

        if (stairParams.goingUp)
        {
            GameManager.instance.floor++;
        }
        else
        {
            GameManager.instance.floor--;
        }

        GameManager.instance.spawnPos = GetStairsMoveToPos();
        //if (GameObject.Find("OWPlayer").transform.localPosition == GameObject.Find("OWPlayer").GetComponent<OWPlayerController>().TargetGridPos)
        //{
        //    return;
        //}

        LevelGenerator.instance.ResetFloors();
        //if(tempFloorNum < GameManager.instance.floor)
        if (stairParams.goingUp)
        {
            LevelGenerator.instance.floorParent.transform.position = new Vector3(0, 2, 0);
        }
        else
        {
            LevelGenerator.instance.floorParent.transform.position = new Vector3(0, -2, 0);
        }

        LevelGenerator.instance.BeginStairsTransition(stairParams.goingUp);
    }

    private Vector3 GetStairsMoveToPos()
    {
        Vector3 pos = GameObject.Find("OWPlayer").transform.localPosition;
        return new Vector3(Mathf.RoundToInt(GameObject.Find("OWPlayer").transform.localPosition.x + (GameObject.Find("OWPlayer").transform.forward.x * 3)), 0f,
            Mathf.RoundToInt(GameObject.Find("OWPlayer").transform.localPosition.z + (GameObject.Find("OWPlayer").transform.forward.z * 3)));
    }
}
public enum FacingDirection { Up = 0, Down = 1, Left = 2, Right = 3 } // For events that spawn meshes, this affects the rotation the mesh spawns in with.
#endregion
#region Ladder
/// <summary>
/// Ladder Event Details
/// The following code is used for Ladders. Some data is taken from the Stairs code above.
/// </summary>
public partial class FloorEvent
{
    public LadderParams ladderParams;
    [System.Serializable]
    public class LadderParams
    {
        public bool changeArea;
        public int area;
        public int floor;
    }

    public void Ladder()
    {
        bool goingUp = false;
        if (ladderParams.changeArea)
        {
            if (ladderParams.area > GameManager.instance.area)
            {
                goingUp = true;
            }
        }
        else if (ladderParams.floor > GameManager.instance.floor)
        {
            goingUp = true;
        }

        if (ladderParams.area != GameManager.instance.area)
        {
            GameManager.instance.area = ladderParams.area;
        }
        GameManager.instance.floor = ladderParams.floor;

        //GameManager.instance.spawnPos = new Vector2(Mathf.RoundToInt(transitionData.moveToPos.x), Mathf.RoundToInt(transitionData.moveToPos.y));

        // Call Ladder Function
        LevelGenerator.instance.BeginLadderTransition(goingUp, ladderParams.area, ladderParams.floor);

        //AdvanceEventQueue();
    }
}
#endregion
#region Rope
/// <summary>
/// Rope Event Details
/// The following code is used for Rope spots.
/// </summary>
public partial class FloorEvent
{
    public RopeParams ropeParams;
    [System.Serializable]
    public class RopeParams
    {
        public int[] floor;
    }

    public void Rope()
    {
        // Bring up a menu that allows the player to move between floors to whichever one is chosen

        AdvanceEventQueue();
    }
}
#endregion
#region Warp
/// <summary>
/// Warp Event Details
/// The following code is used for Warp spots.
/// </summary>
public partial class FloorEvent
{
    public WarpParams warpParams;
    [System.Serializable]
    public class WarpParams
    {
        public WarpData[] warpData;
    }

    public void Warp()
    {
        // Bring up a menu that allows the player to warp to another preset point, or choose between multiple if multiple are included.
        AdvanceEventQueue();
    }
}
[System.Serializable]
public struct WarpData
{
    public int areaNum;
    public int floorNum;

    public Vector2 moveToPos;
    public FacingDirection spawnDir;
}
#endregion
#region Door
/// <summary>
/// Door Event Details
/// The following code is used for Doors.
/// </summary>
public enum DoorType { Normal = 0, OneWay = 1, Gold = 2 }
public partial class FloorEvent
{
    public DoorParams doorParams;
    [System.Serializable]
    public class DoorParams
    {
        public DoorType doorType;

        public bool openDoorLeft = true;
        public bool openDoorRight = true;
        public bool openDoorUp = true;
        public bool openDoorDown = true;
    }

    public void Door()
    {
        RegisterEventToSwitches(true, false);
        OpenDoor();
        AdvanceEventQueue();
    }

    private void OpenDoor()
    {
        if (GameManager.instance.eventSwitches[eventID])
        {
            if (doorParams.doorType == DoorType.Normal)
            {
                if (InventoryManager.instance.GetQuantity("Key") > 0)
                {
                    InventoryManager.instance.ModifyInventory("Key", -1);
                    Unlock();
                }
                else
                {
                    UIManager.instance.messageBoxHandler.PlayMessage("This door requires a key.");
                    //GameManager.instance.soundPlayer.PlayAudio(GameManager.instance.soundPlayer.doorClosedSound);
                }
            }
            if (doorParams.doorType == DoorType.OneWay)
            {
                //check that the player is facing the correct direction
                if (CanOpen())
                {
                    Unlock();
                }
                else
                {
                    UIManager.instance.messageBoxHandler.PlayMessage("This door can't be opened from this side.");
                    //GameManager.instance.soundPlayer.PlayAudio(GameManager.instance.soundPlayer.doorClosedSound);
                }
            }

            LevelGenerator.instance.ResetCurrentFloor();
        }
    }

    private void Unlock()
    {
        //LevelGenerator.instance.floorEvents[(int)mapPos.x, (int)mapPos.y].GetActiveSequence().overwriteWalkable = OverwriteWalkable.Walkable;
        GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().SetTileWalkable((int)mapPos.x, (int)mapPos.y, true);
        UIManager.instance.messageBoxHandler.PlayMessage("Door unlocked.");
        //GameManager.instance.soundPlayer.PlayAudio(GameManager.instance.soundPlayer.doorSound);
        RegisterEventToSwitches(false);
    }

    protected virtual bool CanOpen()
    {
        // Used for one way doors, aka doors that do not need keys
        switch (GameObject.Find("OWPlayer").transform.eulerAngles.y)
        {
            case 90:
            case -270:
                return doorParams.openDoorLeft;
            case -180:
            case 180:
                return doorParams.openDoorUp;
            case -90:
            case 270:
                return doorParams.openDoorRight;
            case 0:
                return doorParams.openDoorDown;
            default:
                return false;
        }
    }
}
#endregion
#region Dialog Scene
/// <summary>
/// Dialog Scene Event Details
/// The following code is used for Dialog Scenes.
/// </summary>
public partial class FloorEvent
{
    public DialogParams dialogParams;
    [System.Serializable]
    public class DialogParams
    {
        public DialogEventScriptableObject dialogEventData;
    }

    public void PlayDialogEvent()
    {
        DialogManager.instance.PlayDialogSet(dialogParams.dialogEventData.dialogSets);
    }
    public void PlaySingleDialogEvent(int index)
    {
        try
        {
            DialogManager.instance.PlayDialog(dialogParams.dialogEventData.dialogSets[index].dialogs);
        }
        catch (System.Exception)
        {
            Debug.LogError("WARNING: DialogManager for this event is null! Did you remember to run SetupDialogManager?");
            throw;
        }
    }
}
#endregion
#region Battle Event
/// <summary>
/// Battle Event Details
/// The following code is used for Battles.
/// </summary>
public partial class FloorEvent
{
    public BattleParams battleParams;
    [System.Serializable]
    public class BattleParams
    {
        public BattleData battleData;
    }

    // The following functions should be moved to the BattleManager, and this should hold a function that calls battleManager.instance.StartBattle();
    public void StartBattle()
    {
        // turn on battle camera
        // turn off dungeon camera

        // battleManager.battleOn = true;

        // turn on BattlePlayer's inputs

        // turn off dungeon UI

        GameManager.instance.battleCamera.gameObject.SetActive(true);
        GameManager.instance.dungeonCamera.gameObject.SetActive(false);

        GameManager.instance.battleManager.battleOn = true;

        GameManager.instance.battlePlayer.GetComponent<BattleInput>().controlLock = false;

        GameManager.instance.SetFrameRate("battle");
    }

    public void EndBattle()
    {
        // turn on dungeon camera
        // turn off battle camera

        // battleManager.battleOn = false;

        // advance event queue

        // turn on dungeon UI

        GameManager.instance.dungeonCamera.gameObject.SetActive(true);
        GameManager.instance.battleCamera.gameObject.SetActive(false);

        GameManager.instance.battleManager.battleOn = false;

        GameManager.instance.SetFrameRate("dungeon");

        AdvanceEventQueue();
    }
}
#endregion
#region Item
/// <summary>
/// Item Event Details
/// The following code is used for Item events. These allow the player to gain or lose items.
/// </summary>
public partial class FloorEvent
{
    public ItemParams itemParams;
    [System.Serializable]
    public class ItemParams
    {
        public string itemName;
        public int itemQuantity; // How many of the corresponding type of item should be added to the player's supply. Negative numbers will subtract items. The actual item count will be clamped at 0-99.
    }

    public void ItemGet()
    {
        // increase the item of typeOfItem in the player's inventory by itemQuantity
        InventoryManager.instance.ModifyInventory(itemParams.itemName, itemParams.itemQuantity);
        AdvanceEventQueue();
    }
}
#endregion
#region Conditional
/// <summary>
/// Conditional Event Details
/// The following code is used for Conditionals. Criteria is tracked, then one of two event sequences is taken depending on if that criteria is true.
/// </summary>
public partial class FloorEvent
{
    public class ConditionalParams
    {

    }
}
#endregion
#endregion

public partial class FloorEvent
{
    public FloorEvent CopyFloorEvent()
    {
        FloorEvent copyEvent = new FloorEvent();

        copyEvent.typeOfEvent = this.typeOfEvent;

        copyEvent.stairParams = this.stairParams;
        copyEvent.ladderParams = this.ladderParams;
        copyEvent.ropeParams = this.ropeParams;
        copyEvent.warpParams = this.warpParams;
        copyEvent.doorParams = this.doorParams;
        copyEvent.dialogParams = this.dialogParams;
        copyEvent.battleParams = this.battleParams;
        copyEvent.itemParams = this.itemParams;

        return copyEvent;
    }
}
using UnityEngine;

// All this script is is a container for a FloorEvent that can be used in place of a manually made FloorEvent. Useful for common events.
[CreateAssetMenu(fileName = "commonevent_0_1", menuName = "CommonEvent")]
public class FloorEventScriptableObject : ScriptableObject
{
    public FloorEvent fEvent;
}

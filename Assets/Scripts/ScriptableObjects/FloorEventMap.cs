using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "eventmap_0_0", menuName = "Floor Event Map")]
public class FloorEventMap : ScriptableObject
{
    public Texture2D floorMap;

    //public FloorEvent[] floorEvents;
    public EventBranch[] eventBranches;
}

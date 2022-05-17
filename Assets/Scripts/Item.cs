using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE: Change this whole script to a serializable object later.
/// </summary>

public enum ItemCategory { Normal = 0, Key = 1, Equipment = 2 }
[System.Serializable]
public class Item
{
    public ItemCategory itemCategory;

    public string itemName;

    public int itemCap; // If set to 0 or a negative number, cap will be 99.
}

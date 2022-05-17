using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemset_0_0", menuName = "ItemSet")]
public class ItemSet : ScriptableObject
{
    public List<Item> itemList;
}

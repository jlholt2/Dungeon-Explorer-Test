using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    //public List<Item> itemList;
    public ItemSet itemSet;

    public Dictionary<string, Item> inventory = new Dictionary<string, Item>();
    public Dictionary<Item, int> inventoryQuantities = new Dictionary<Item, int>();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetupInventory();
    }

    public void SetupInventory()
    {
        if (itemSet.itemList.Count < 1)
        {
            return;
        }
        foreach(Item itemData in itemSet.itemList)
        {
            RegisterItem(itemData);
        }
    }

    public void ModifyInventory(string itemName, int count)
    {
        ModifyInventory(itemName, count, true);
    }
    public void ModifyInventory(string itemName, int count, bool add) // modify this later to support arrays (or better yet make a complimentary function to support arrays)
    {
        if (add)
        {
            inventoryQuantities[inventory[itemName]] = inventoryQuantities[inventory[itemName]] + count;
        }
        else
        {
            inventoryQuantities[inventory[itemName]] = count;
        }
    }

    public int GetQuantity(string itemName)
    {
        if (ItemExists(itemName))
        {
            return inventoryQuantities[inventory[itemName]];
        }
        return 0;
    }

    public bool ItemExists(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            return true;
        }return false;
    }

    public void RegisterItem(Item itemData) // change this later to use an item class instead, for better setting of items (maybe consider serializable objects?)
    {
        if (!ItemExists(itemData.itemName))
        {
            inventory.Add(itemData.itemName, itemData);
            inventoryQuantities.Add(inventory[itemData.itemName], 0);
        }
    }
}

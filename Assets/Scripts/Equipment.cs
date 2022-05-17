using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType { Weapon = 0, Head = 1, Body = 2, Hands = 3, Feet = 4, Accesory = 5 }
public class Equipment : Item
{
    public EquipmentType equipType;
}

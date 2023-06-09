using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unity note: useful how enum like this is exposed to inspector as dropdown.
public enum ItemType
{
    Health,
    Key,
    Clothing,
    InteractivePickup,
    Default
}

// Abstract to stay extendable to diff ItemObject types
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab; // changed to sprite in v2
    public ItemType type;
    //public bool objectiveTrigger = false;
    //public string objectiveID;
    [TextArea(15, 20)]
    public string description;
}
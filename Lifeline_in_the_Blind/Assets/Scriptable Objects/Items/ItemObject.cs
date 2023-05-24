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

// EXTENSIONS OF ABOVE PARENT CLASS ----------------------------------------------------

[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
public class DefaultObject : ItemObject
{
    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.Default;
    }
}

[CreateAssetMenu(fileName = "New Interactive Pickup", menuName = "Inventory System/Items/Interactive Pickup")]
public class InteractivePickup : ItemObject
{
    public string pickupPrompt;

    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.Health;
        pickupPrompt = "Press 'E' to pick up item";
    }
}

[CreateAssetMenu(fileName = "New Clothing Object", menuName = "Inventory System/Items/Clothing")]
public class ClothingObject : ItemObject
{
    // Review This could also be implemented as an enum perhaps.
    public int clothingProtectionValue;

    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.Clothing;
    }
}

// Extends the ItemObject class.
[CreateAssetMenu(fileName = "New Health Object", menuName = "Inventory System/Items/Health")]
public class HealthObject : ItemObject
{
    public int restoreHealthValue;

    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.Health;
    }
}

// Extends the ItemObject class.
[CreateAssetMenu(fileName = "New Key Object", menuName = "Inventory System/Items/Key")]
public class KeyObject : ItemObject
{
    // FIXME specify door in some way which key opens.
    // Just a GameObject reference?

    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.Key;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

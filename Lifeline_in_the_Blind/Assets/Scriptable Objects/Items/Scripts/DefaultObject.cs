using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extends the ItemObject class.
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

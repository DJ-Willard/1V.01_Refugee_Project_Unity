using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
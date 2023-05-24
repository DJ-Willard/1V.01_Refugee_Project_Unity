using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Interactive Pickup", menuName = "Inventory System/Items/Interactive Pickup")]
public class InteractivePickup : ItemObject
{
    public string pickupPrompt;

    // When object is created from right click menu in Project browser, automatically set
    // its item type appropriately.
    public void Awake()
    {
        type = ItemType.InteractivePickup;
        pickupPrompt = "Press 'E' to pick up item";
    }
}
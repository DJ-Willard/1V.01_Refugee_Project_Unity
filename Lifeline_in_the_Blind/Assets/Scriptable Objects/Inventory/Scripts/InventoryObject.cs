using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    // Could implement as dictionary instead, going with List for the moment.
    // renamed from 'Container' to 'InventoryList'
    public List<InventorySlot> InventoryList = new List<InventorySlot>();

    // If Container list already has item, just add to amount and return,
    // otherwise loop exits without return and add new item and amount.
    public void AddItem(ItemObject _item, int _amount)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].item == _item)
            {
                InventoryList[i].AddAmount(_amount);
                return;
            }
        }
        InventoryList.Add(new InventorySlot(_item, _amount));
    }

    // UNTESTED
    public void RemoveItem(ItemObject _item, int _amount)
    {
        bool itemFound = false;
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].item == _item)
            {
                InventoryList[i].RemoveAmount(_amount);
                itemFound = true;
                if (InventoryList[i].amount == 0)
                {
                    InventoryList.RemoveAt(i);
                }
                break;
            }
        }
        if (!itemFound)
        {
            Debug.LogError("Error: Item to be removed from inventory was not found.");
        }
    }
}

[System.Serializable]   // Expose class to Unity editor
public class InventorySlot
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public void RemoveAmount(int value)
    {
        amount -= value;
    }
}
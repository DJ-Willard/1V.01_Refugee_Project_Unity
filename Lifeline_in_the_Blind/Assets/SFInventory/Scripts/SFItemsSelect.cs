using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFInventory;
using System;

namespace SFInventory.Demo
{
    //this script is used to get an item from the cell where the item was moved to
    public class SFItemsSelect : MonoBehaviour
    {
        private SFInventoryCell cell;

        private void Start()
        {
            cell = GetComponent<SFInventoryCell>();
            cell.inventoryManager.onEndDrop += OnEndDrop;
        }

        private void OnEndDrop(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            if (dropCell == cell && cell.item != null)
            {
                Debug.Log(dropCell.item.name);
            }
        }

        //adding callbacks
        private void OnEnable()
        {
            if (cell && cell.inventoryManager)
                cell.inventoryManager.onEndDrop += OnEndDrop;
        }
        //removing callbacks
        private void OnDisable()
        {
            if (cell && cell.inventoryManager)
                cell.inventoryManager.onEndDrop -= OnEndDrop;
        }
    }
}
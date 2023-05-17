using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFInventory
{
    public class SFInventoryManager : MonoBehaviour
    {
        //all cells that belong to the inventory 
        public List<SFInventoryCell> inventoryCells = new List<SFInventoryCell>();

        public Sprite noItemIcon;
        public SFInventoryItem testItem;

        public Action<SFInventoryCell> onBeginDrag;
        public Action<SFInventoryCell> onEndDrag;
        public Action<SFInventoryCell, SFInventoryCell> onDrop;
        public Action<SFInventoryCell, SFInventoryCell> onEndDrop;
        public Action<SFInventoryCell, SFInventoryCell> onDivide;
        public Action<SFInventoryCell> onDrag;

        [HideInInspector] public SFInventoryCell currentEnteredCell;

        //adding new item to free cell
        public void AddItem(SFInventoryItem item, int amount)
        {
            //checking for a free cell
            if (GetFreeCell(out SFInventoryCell cell))
            {
                cell.SetItem(item, amount);
            }
            else
            {
                Debug.Log("Not enough space");
            }
        }

        //more complicated adding new item to cell
        public void AddItemsCount(SFInventoryItem item, int amount, out int itemsLeft)
        {
            itemsLeft = amount;
            //checking for the presence of an item with the same type or a free cell
            if (GetFreeCellWithSpace(item, out SFInventoryCell cell) || GetFreeCell(out cell))
            {
                //simple math to add new items in quantity 
                int diff = item.MaximumItemsCount - cell.itemsCount;
                if (diff < itemsLeft)
                {
                    cell.SetItem(item, item.MaximumItemsCount);
                    itemsLeft -= diff;
                    //recursion with which several cells are added if everything does not fit in one cell
                    AddItemsCount(item, itemsLeft, out itemsLeft);
                }
                else
                {
                    cell.SetItem(item, itemsLeft + cell.itemsCount);
                    itemsLeft = 0;
                }
            }
        }

        //get a free cell that does not contain any items
        public bool GetFreeCell(out SFInventoryCell cell)
        {
            return GetFreeCellByItem(null, out cell);
        }

        //get a free cell that contain item of type with some space in it
        public bool GetFreeCellWithSpace(SFInventoryItem item, out SFInventoryCell cell)
        {
            cell = inventoryCells.FirstOrDefault(val => val.item == item && val.itemsCount < val.item.MaximumItemsCount && val.dropItemOnAdd == true);
            return cell;
        }

        //get a free cell that contain item of type
        public bool GetFreeCellByItem(SFInventoryItem item, out SFInventoryCell cell)
        {
            cell = inventoryCells.FirstOrDefault(val => val.item == item && val.dropItemOnAdd == true);
            return cell;
        }

        //OnBeginDrag callback
        public void OnBeginDrag(SFInventoryCell dragCell)
        {
            onBeginDrag?.Invoke(dragCell);
        }

        //OnEndDrag callback
        public void OnEndDrag(SFInventoryCell dragCell)
        {
            onEndDrag?.Invoke(dragCell);
        }

        //OnDrag callback
        public void OnDragCell(SFInventoryCell dragCell)
        {
            onDrag.Invoke(dragCell);
        }

        //OnDrop callback
        public void OnDropCell(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            onDrop?.Invoke(dragCell, dropCell);
        }

        //OnDivide callback
        public void OnDivideCell(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            onDivide?.Invoke(dragCell, dropCell);
        }
    }
}
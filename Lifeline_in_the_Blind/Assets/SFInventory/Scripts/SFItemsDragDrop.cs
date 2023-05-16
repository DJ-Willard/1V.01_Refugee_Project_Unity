using System;
using UnityEngine;

namespace SFInventory
{
    public class SFItemsDragDrop : MonoBehaviour
    {
        [SerializeField] private GameObject dragCell;
        private MouseDragCell mouseDragCell;
        private SFInventoryManager inventoryManager;

        //a set of conditions for moving an item from one cell to another
        private void OnDrop(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            if (dragCell == dropCell)
                return;

            if (dropCell.typeMatch && dragCell.item?.ItemType != dropCell.cellItemType)
                return;

            if (dragCell.item == dropCell.item)
            {
                int diff = (dragCell.itemsCount + dropCell.itemsCount) - dragCell.item.MaximumItemsCount;

                if (dropCell.itemsCount + dragCell.itemsCount > dragCell.item.MaximumItemsCount && !(dropCell.itemsCount >= dropCell.item.MaximumItemsCount))
                {
                    dropCell.itemsCount = dropCell.item.MaximumItemsCount;
                    dragCell.itemsCount = diff;
                }
                else if (dropCell.itemsCount + dragCell.itemsCount <= dragCell.item.MaximumItemsCount && !(dropCell.itemsCount >= dropCell.item.MaximumItemsCount))
                {
                    dropCell.itemsCount += dragCell.itemsCount;
                    dragCell.ClearCell();
                }
                else
                {
                    ReplaceItems(dragCell, dropCell);
                }
            }
            else
            {
                ReplaceItems(dragCell, dropCell);
            }
            dragCell?.UpdateCell();
            dropCell?.UpdateCell();
            inventoryManager.onEndDrop?.Invoke(dragCell, dropCell);
        }

        //Swap items in a cell
        private void ReplaceItems(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            (dropCell.item, dragCell.item) = (dragCell.item, dropCell.item);
            (dropCell.itemsCount, dragCell.itemsCount) = (dragCell.itemsCount, dropCell.itemsCount);
            dragCell?.UpdateCell();
            dropCell?.UpdateCell();
        }

        //disable mouseDragCell when cell drag is stopped
        private void OnEndDrag(SFInventoryCell cell)
        {
            mouseDragCell.gameObject.SetActive(false);
        }

        //initializing mouseDragCell
        private void OnBeginDrag(SFInventoryCell cell)
        {
            if (cell.item)
            {
                mouseDragCell.Init(cell);
                mouseDragCell.gameObject.SetActive(true);
            }
        }

        //changing mouseDragCell position
        private void OnDrag(SFInventoryCell cell)
        {
            if (mouseDragCell.cell)
            {
                mouseDragCell.transform.position = Input.mousePosition;
            }
        }

        //set of conditions for cell division in half
        private void OnDivide(SFInventoryCell dragCell, SFInventoryCell dropCell)
        {
            if (dragCell == dropCell)
                return;

            if (dropCell.typeMatch && dragCell.item?.ItemType != dropCell.cellItemType)
                return;

            if ((dragCell.item == dropCell.item || dropCell.item == null) && dragCell.itemsCount > 1)
            {
                int half = dragCell.itemsCount / 2;

                dropCell.item = dragCell.item;
                if (dropCell.itemsCount + half < dragCell.item.MaximumItemsCount)
                {
                    dragCell.itemsCount -= half;
                    dropCell.itemsCount += half;
                }
                else
                {
                    dragCell.itemsCount -= dropCell.item.MaximumItemsCount - dropCell.itemsCount;
                    dropCell.itemsCount = dropCell.item.MaximumItemsCount;
                }
            }
            if ((dragCell.itemsCount == 1 && !dropCell.item) || dropCell.item != dragCell.item)
            {
                OnDrop(dragCell, dropCell);
            }

            dragCell?.UpdateCell();
            dropCell?.UpdateCell();
        }

        //adding callbacks
        private void OnEnable()
        {
            inventoryManager = GetComponent<SFInventoryManager>();

            if (!mouseDragCell)
                mouseDragCell = Instantiate(dragCell, transform).GetComponent<MouseDragCell>();

            inventoryManager.onDrag += OnDrag;
            inventoryManager.onBeginDrag += OnBeginDrag;
            inventoryManager.onEndDrag += OnEndDrag;
            inventoryManager.onDrop += OnDrop;
            inventoryManager.onDivide += OnDivide;

            mouseDragCell.gameObject.SetActive(false);
        }

        //removing callbacks
        private void OnDisable()
        {
            inventoryManager.onDrag -= OnDrag;
            inventoryManager.onBeginDrag -= OnBeginDrag;
            inventoryManager.onEndDrag -= OnEndDrag;
            inventoryManager.onDrop -= OnDrop;
            inventoryManager.onDivide -= OnDivide;
        }

    }
}
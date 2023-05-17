using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SFInventory
{
    //adding interfaces for working with ui and callbacks
    public class SFInventoryCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image icon;
        public Text itemsCountText;
        public bool typeMatch = false;
        public ItemType cellItemType;
        public bool dropItemOnAdd = true;
        [HideInInspector] public SFInventoryItem item;
        [HideInInspector] public int itemsCount;
        [HideInInspector] public SFInventoryManager inventoryManager;

        //indicate which inventory the cell belongs to
        public void InitCell(SFInventoryManager inventoryManager)
        {
            this.inventoryManager = inventoryManager;
        }
        //OnBeginDrag callback in inventory
        public void OnBeginDrag(PointerEventData eventData)
        {
            inventoryManager.OnBeginDrag(this);
        }
        
        //OnDrag callback in inventory
        public void OnDrag(PointerEventData eventData)
        {
            inventoryManager.OnDragCell(this);
        }

        //OnEndDrag callback and OnDivide callback in inventory
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryManager.OnEndDrag(this);
            if (inventoryManager.currentEnteredCell)
            {
                if (eventData.button == PointerEventData.InputButton.Middle)
                    inventoryManager.OnDivideCell(this, inventoryManager.currentEnteredCell);
                else
                    inventoryManager.OnDropCell(this, inventoryManager.currentEnteredCell);
            }
        }

        //indicate which cell the cursor is currently hovering over
        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryManager.currentEnteredCell = this;
        }

        //remove the cell when the mouse leaves it
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryManager.currentEnteredCell = null;
        }

        //placing an item in cell
        public void SetItem(SFInventoryItem item, int amount)
        {
            this.item = item;
            itemsCount = amount;
            UpdateCell();
        }

        //removing item from cell
        public void ClearCell()
        {
            item = null;
            itemsCount = 0;
            UpdateCell();
        }

        //update the cell i.e. set the icon and other components that you may add
        public void UpdateCell()
        {
            icon.sprite = item?.Icon ?? inventoryManager.noItemIcon;

            if (itemsCountText)
            {
                if (itemsCount > 1)
                    itemsCountText.text = itemsCount.ToString();
                else
                    itemsCountText.text = "";
            }
        }
    }
}
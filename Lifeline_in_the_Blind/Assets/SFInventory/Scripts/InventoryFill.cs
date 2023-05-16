using UnityEngine;
using SFInventory;

namespace SFInventory.Demo
{
    public class InventoryFill : MonoBehaviour
    {
        [SerializeField] private int width = 9;
        [SerializeField] private int height = 5;
        [SerializeField] private GameObject inventoryCell;
        [SerializeField] private SFInventoryManager inventoryManager;

        void Awake()
        {
            Fill();
        }

        private void Fill()
        {
            //adding cells that were already on the stage
            for (int i = 0; i < inventoryManager.inventoryCells.Count; i++)
            {
                inventoryManager.inventoryCells[i].InitCell(inventoryManager);
                inventoryManager.inventoryCells[i].UpdateCell();
            }
            //simple filling of cells with the parent of the current object and adding them to the inventory system
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject item = Instantiate(inventoryCell, transform);
                    SFInventoryCell cell = item.GetComponent<SFInventoryCell>();
                    cell.InitCell(inventoryManager);
                    cell.UpdateCell();
                    inventoryManager.inventoryCells.Add(cell);
                }
            }
        }
    }
}
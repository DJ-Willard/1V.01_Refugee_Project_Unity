using UnityEngine;
using UnityEngine.UI;

namespace SFInventory
{
    public class MouseDragCell : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [HideInInspector] public SFInventoryCell cell;

        //initialization of an item in a dragged cell for further work with it
        public void Init(SFInventoryCell cell)
        {
            this.cell = cell;
            icon.sprite = cell.item.Icon;
        }
    }
}
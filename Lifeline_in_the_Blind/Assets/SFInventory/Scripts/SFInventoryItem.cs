using UnityEngine;

namespace SFInventory
{
    //an item in which all the settings and other objects are located for further work with them
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "SFInventory/InventoryItem", order = 1)]
    public class SFInventoryItem : ScriptableObject
    {
        public Sprite Icon;
        public string Name;
        public int MaximumItemsCount = 64;
        public ItemType ItemType;
    } 
    
    //item type
    public enum ItemType
    {
        Food, Weapon, Stuff, HeadArmor, BodyArmor
    }
}
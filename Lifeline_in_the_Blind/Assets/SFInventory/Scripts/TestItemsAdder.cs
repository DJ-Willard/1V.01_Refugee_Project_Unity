using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFInventory;

namespace SFInventory.Demo
{
    public class TestItemsAdder : MonoBehaviour
    {
        public SFInventoryItem[] testItems;
        public int addCount = 5;
        private SFInventoryManager inventoryManager;

        void Start()
        {
            inventoryManager = GetComponent<SFInventoryManager>();
        }

        void Update()
        {
            //here all the numbers of the button that are pressed will be converted to an integer and with this number you will add the item to your inventory
            if (Input.anyKey)
            {
                // Debug PW
                // Debug.Log(Input.inputString);
                // this may not play nice with preexisting input system
                
                if (int.TryParse(Input.inputString, out int i) && i <= testItems?.Length && i > 0)
                {
                    inventoryManager.AddItemsCount(testItems[i - 1], addCount, out int left);
                    //if there is not enough space in your inventory, you will get the remaining number of items back
                    if (left > 0)
                        Debug.Log("Inventory overflow: " + left + " " + testItems[i - 1].Name);
                }
            }
        }
    }
}
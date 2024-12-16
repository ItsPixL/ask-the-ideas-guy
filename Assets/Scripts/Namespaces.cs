using UnityEngine;
using System.Collections.Generic;

namespace InventoryManager {
    public class Item {
        public string name;
        public string description;
        public Item(string name, string description) {
            this.name = name;
            this.description = description;
        }
    }

    public class Inventory {
        public List<Item> items;
        private int maxSlots;
        private int currItems = 0;
        public int currIdx = 0;
        public Item currItem;

        public Inventory(int maxSlots) {
            this.maxSlots = maxSlots;
        }

        // Empties player inventory.
        public void resetInventory() { 
            items = new List<Item>(new Item[maxSlots]);
        }

        private int keepIdxInRange(int currIdx) {
            if (currIdx < 0) {
                return maxSlots+currIdx;
            }
            if (currIdx >= maxSlots) {
                return maxSlots-currIdx;
            }
            return currIdx;
        } 

        // Manages inventory navigation by key inputs.
        public void navigateInventory() {
            if (Input.GetKeyDown("e")) {
                currIdx += 1;
            }
            if (Input.GetKeyDown("q")) {
                currIdx -= 1;
            }
            for (int i = 1; i <= maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i))) {
                    currIdx = i-1;
                    // Debug.Log(currIdx);
                }
            }
            currIdx = keepIdxInRange(currIdx);
            fetchCurrItem(currIdx);
        }

        // Fetches the current item being used.
        public void fetchCurrItem(int targetIdx) {
            currIdx = targetIdx;
            if (items[targetIdx] == null) {
                currItem = null;
            }
            else {
                currItem = items[targetIdx];
            }
        }
    }
}

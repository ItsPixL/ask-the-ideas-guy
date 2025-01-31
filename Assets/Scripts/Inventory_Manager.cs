using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


namespace InventoryManager {
    public class Item {
        public string name;
        public string description;
        public Image image;
        public Item(string name, string description, Image image) {
            this.name = name;
            this.description = description;
            this.image = image;
        }
    }

    public class Inventory {
        public List<Item> items;
        private int maxSlots;
        private int currItems = 0;
        public int currIdx;
        public bool selectedSlot = false;
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
        public bool navigateInventory() {
            int prevIdx = currIdx;
            bool keyPressed = false;
            bool numberPressed = false;
            if (selectedSlot) {
                if (Input.GetKeyDown("e")) {
                    currIdx += 1;
                    currIdx = keepIdxInRange(currIdx);
                    keyPressed = true;
                }
                if (Input.GetKeyDown("q")) {
                    currIdx -= 1;
                    currIdx = keepIdxInRange(currIdx);
                    keyPressed = true;
                }
            }
            for (int i = 1; i <= maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i))) {
                    currIdx = i-1;
                    numberPressed = true;
                    keyPressed = true;
                }
            }
            if (numberPressed && prevIdx == currIdx) {
                selectedSlot = !selectedSlot;
                // Debug.Log(selectedSlot);
            }
            else if (prevIdx != currIdx) {
                fetchCurrItem(currIdx);
            }
            return keyPressed;
        }

        // Fetches the current item being used.
        public void fetchCurrItem(int targetIdx) {
            if (targetIdx == -1) {
                currItem = null;
                selectedSlot = false;
            }
            else {
                currIdx = targetIdx;
                if (items[targetIdx] == null) {
                    currItem = null;
                }
                else {
                    currItem = items[targetIdx];
                }
                selectedSlot = true;
                // Debug.Log(currIdx);
            }
        }

        public void addItem(Item item) {
            if (currItems < maxSlots) {
                items.Add(item);
                currItems += 1;
            }
        }

        public void deleteItem(int itemIdx) {
            items[itemIdx] = null;
            currItems -= 1;
        }
    }

    public class UI_Inventory {
        private List<Button> buttons;
        public int currSelected = -1;
        public UI_Inventory(List<Button> buttons) {
            this.buttons = buttons;
        }
        public void selectCurrItem(int newIdx) {
            Debug.Log(currSelected);
            Debug.Log(newIdx);
            if (currSelected != -1) {
                Button prevButton = buttons[currSelected];
                Outline prevOutline = prevButton.GetComponent<Outline>();
                prevOutline.effectColor = Color.black;
            }
            if (newIdx == currSelected) {
                currSelected = -1;
            }
            else {
                currSelected = newIdx;
            }
            if (currSelected != -1) {
                Button currButton = buttons[currSelected];
                Outline currOutline = currButton.GetComponent<Outline>();
                currOutline.effectColor = Color.yellow;
            }
            Debug.Log(currSelected);
        }
    }
}

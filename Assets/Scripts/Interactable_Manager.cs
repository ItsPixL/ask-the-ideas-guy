using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace InteractableManager {
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

    public class Ability {
        public string name;
        public string description;
        public int energyCost;
        public int cooldown;
        public Image image;
        public Ability(string name, string description, int energyCost, int cooldown, Image image) {
            this.name = name;
            this.description = description;
            this.energyCost = energyCost;
            this.cooldown = cooldown;
            this.image = image;
        }
    }

    public class Inventory {
        public List<Item> items;
        public List<int> numberShortcuts;
        private int maxSlots;
        private int currItems = 0;
        public int currIdx;
        public bool selectedSlot = false;
        public Item currItem;

        public Inventory(int maxSlots, List<int> numberShortcuts) {
            this.maxSlots = maxSlots;
            this.numberShortcuts = numberShortcuts;
        }

        // Empties player inventory.
        public void resetInventory() { 
            items = new List<Item>(new Item[maxSlots]);
        }

        // Manages inventory navigation by key inputs.
        public bool checkKeyInput() {
            int prevIdx = currIdx;
            bool keyPressed = false;
            bool numberPressed = false;
            for (int i = 0; i < maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + numberShortcuts[i]))) {
                    currIdx = i;
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

        // Adds an item to the player inventory (if the inventory is full, replaces the currently selected item with the new item).
        public void addItem(Item item) {
            if (currItems < maxSlots) {
                for (int i = 0; i < maxSlots; i++) {
                    if (items[i] is null) {
                        items[i] = item;
                    }
                }
                currItems += 1;
            }
            else {
                items[currIdx] = item;
            }
        }

        // Removes an item from the player inventory.
        public void removeItem(int itemIdx) {
            items[itemIdx] = null;
            currItems -= 1;
        }
    }

    public class Loadout {
        public List<Ability> abilities;
        public List<int> numberShortcuts;
        public List<int> currSlotsUsed = new List<int>();
        private int maxSlots;

        public Loadout(int maxSlots, List<int> numberShortcuts) {
            this.maxSlots = maxSlots;
            this.numberShortcuts = numberShortcuts;
        }

        // Removes all abilities from player.
        public void resetLoadout() { 
            abilities = new List<Ability>(new Ability[maxSlots]);
        }

        // Checks whether a player can use a given ability.
        public bool canUseAbility(int abilityIdx, int currEnergy) {
            if (abilities[abilityIdx] is not null && currEnergy < abilities[abilityIdx].energyCost) {
                return true;
            }
            return false;
        }

        // Uses an ability if the player has the energy required.
        public int useAbility(int abilityIdx, int currEnergy) {
            // The code for using the ability will be placed here when the ability code is ready.
            return currEnergy-abilities[abilityIdx].energyCost;
        }

        // Checks whether the player has pressed one of the number shortcuts (which can cause an ability to be used).
        public int checkKeyInput() {
            int keyPressed = -1;
            for (int i = 0; i < maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + numberShortcuts[i]))) {
                    keyPressed = i;
                }
            }
            return keyPressed;
        }

        // Adds an ability that the player can use and record the slot that it is in.
        public void addAbility(Ability ability) {
            for (int i = 0; i < maxSlots; i++) {
                if (abilities[i] is null) {
                    abilities[i] = ability;
                    currSlotsUsed.Add(i);
                }
            }
        }

        // Removes an ability that that player can use and remove that index from the list of used slots.
        public void removeAbility(int targetIdx) {
            if (currSlotsUsed.Exists(idx => idx == targetIdx)) {
                currSlotsUsed.Remove(targetIdx);
                abilities[targetIdx] = null;
            }
        }
    }

    public class UI_Inventory {
        private List<Button> buttons;
        public int currSelected = -1;
        public UI_Inventory(List<Button> buttons) {
            this.buttons = buttons;
        }

        // Highlights the outline of the selected item slot (if any) in yellow, and leave the rest of the outlines black.
        public void selectCurrItem(int newIdx) {
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
        }
    }
}

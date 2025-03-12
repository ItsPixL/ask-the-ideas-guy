using UnityEngine;
using System.Collections.Generic;

namespace InteractableManager {
    // Defines the basics of every item in the game.
    public class Item {
        public string name;
        public Sprite object2D;
        public GameObject object3D;
        public List<Ability> abilityList;
        public bool withPlayer = false;

        public Item(string name, Sprite object2D, GameObject object3D, List<Ability> abilityList) {
            this.name = name;
            this.object2D = object2D;
            this.object3D = object3D;
            this.abilityList = abilityList;
        }

        public void pickItem() {
            withPlayer = true;
        }

        public void dropItem(Vector3 dropPos, Quaternion rotation) {
            GameObject newObject = Object.Instantiate(object3D, dropPos, rotation);
            Pick_Mechanic objectPickup = newObject.AddComponent<Pick_Mechanic>();
            objectPickup.itemRef = this;
            withPlayer = false;
        }
    }

    // Defines the basics of every ability in the game.
    public class Ability {
        public string name;
        public int cooldown;
        public bool onCooldown = false;
        public Sprite icon;

        public Ability(string name, int cooldown, Sprite icon) {
            this.name = name;
            this.cooldown = cooldown;
            this.icon = icon;
        }

        public virtual bool useAbility(GameObject player) {
            return false;
        }
    }

    // Handles the logistics of the inventory (but not the UI). The inventory stores the items.
    public class Inventory {
        public List<Item> items;
        public List<int> numberShortcuts;
        private int maxSlots;
        private int currItemCount = 0;
        public int currIdx = 0;
        public bool selectedSlot = false;

        public Inventory(int maxSlots, List<int> numberShortcuts) {
            this.maxSlots = maxSlots;
            this.numberShortcuts = numberShortcuts;
        }

        // Empties player inventory.
        public void resetInventory() { 
            items = new List<Item>(new Item[maxSlots]);
        }

        // Manages inventory navigation by key inputs.
        public int checkKeyInput() {
            int numberPressed = -1;
            for (int i = 0; i < maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + numberShortcuts[i]))) {
                    numberPressed = i;
                }
            }
            return numberPressed;
        }

        // Selects the current item being used.
        public void selectCurrItem(int targetIdx) {
            if (targetIdx == -1) {
                selectedSlot = false;
            }
            else {
                currIdx = targetIdx;
                selectedSlot = true;
            }
        }

        // Returns whether there is space (at least one empty slot) in the player inventory.
        public bool spaceInInventory() {
            return currItemCount < maxSlots;
        }

        // Adds an item to the player inventory.
        public void addItem(Item item) {
            if (currItemCount < maxSlots) {
                for (int i = 0; i < maxSlots; i++) {
                    if (items[i] is null) {
                        items[i] = item;
                        currIdx = i;
                        break;
                    }
                }
                currItemCount += 1;
            }
        }

        // Removes an item from the player inventory.
        public void removeItem(int itemIdx) {
            items[itemIdx] = null;
            currItemCount -= 1;
        }
    }

    // Handles the logistics of the loadout (but not the UI). The loadout stores the usable abilities.
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

        // Uses an ability if the player has the energy required.
        public bool useAbility(int abilityIdx, GameObject player) {
            if (abilities[abilityIdx] is not null && !abilities[abilityIdx].onCooldown && abilities[abilityIdx].useAbility(player)){
                abilities[abilityIdx].onCooldown = true;
                return true;
            }
            return false;
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
        public int addAbility(Ability ability, bool forItem) {
            for (int i = 0; i < maxSlots; i++) {
                if (abilities[i] is null) {
                    abilities[i] = ability;
                    if (forItem) {
                        currSlotsUsed.Add(i);
                    }
                    return i;
                }
            }
            return 0;
        }

        // Removes all abilities associated with an item that that player can use and remove those indexes from the list of used slots.
        public void removeAbilities() {
            foreach (int idx in currSlotsUsed) {
                abilities[idx] = null;
            }
            currSlotsUsed = new List<int>();
        }
    }
}
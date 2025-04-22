using UnityEngine;
using System.Collections.Generic;

namespace InteractableManager {
    // Defines the basics of every item in the game.
    public class NPCInteractable {
        public string npcName; // The name of the NPC
        public string npcDialogue; // The dialogue of the NPC

        private void Start() {
            // Initialization code can go here if needed
        }

        private void Update() {
            // Update code can go here if needed
        }

        public void Interact() {
            // Code to handle interaction with the NPC, such as displaying dialogue
            Debug.Log($"Interacting with {npcName}: {npcDialogue}");
        }
    }
    public abstract class Item { // abstract = the class can't be used to create objects, but can be used to create other classes
        public string name;
        public Sprite object2D;
        public GameObject object3D;
        public bool withPlayer = false;

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

    public class Weapon : Item {
        public List<Ability> abilityList;
        public Weapon(string name, Sprite object2D, GameObject object3D, List<Ability> abilityList) {
            this.name = name;
            this.object2D = object2D;
            this.object3D = object3D;
            this.abilityList = abilityList;
        }
    }

    public class Powerup : Item {
        // public int link; // what weapon the powerup is linked (found by the value of the weapon's index in the loadout)
        public int buff; // what buff the powerup gives (found by the value of the weapon's index in the loadout)
        public Powerup(string name, Sprite object2D, GameObject object3D, int buff) {
            this.name = name;
            this.object2D = object2D;
            this.object3D = object3D;
            this.buff = buff;
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

    public class Inventory {
        public List<Item> items;
        public List<int> numberShortcuts;
        protected int maxSlots;
        protected int currItemCount = 0;
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
        public virtual int checkKeyInput() {
            int numberPressed = -1;
            for (int i = 0; i < maxSlots; i++) {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + numberShortcuts[i]))) {
                    numberPressed = i;
                }
            }
            return numberPressed;
        }

        // Selects the current Weapon being used.
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

        // Adds an Weapon to the player inventory.
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

        // Removes an Weapon from the player inventory.
        public void removeItem(int WeaponIdx) {
            items[WeaponIdx] = null;
            currItemCount -= 1;
        }
    }

    public class PowerupInventory: Inventory {
        public List<char> characterShortcuts;
        public PowerupInventory(int maxSlots, List<char> characterShortcuts): base(maxSlots, new List<int>()) {
            this.characterShortcuts = characterShortcuts;
        }

        public override int checkKeyInput() {
            int idxPressed = -1;
            for (int i=0; i < maxSlots; i++) {
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), characterShortcuts[i].ToString().ToUpper());
                if (Input.GetKeyDown(keyCode)) {
                    idxPressed = i;
                }
            }
            return idxPressed;
        }

        // used in the swap powerup logic.
        public void swapPowerup(int PowerupIdx, Powerup powerup) {
            items[PowerupIdx] = powerup;
        }

        // Removes an Powerup from the player inventory.
        public void removePowerup(int PowerupIdx) {
            items[PowerupIdx] = null;
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
        public int addAbility(Ability ability, bool forWeapon) {
            for (int i = 0; i < maxSlots; i++) {
                if (abilities[i] is null) {
                    abilities[i] = ability;
                    if (forWeapon) {
                        currSlotsUsed.Add(i);
                    }
                    return i;
                }
            }
            return 0;
        }

        // Removes all abilities associated with an Weapon that that player can use and remove those indexes from the list of used slots.
        public void removeAbilities() {
            foreach (int idx in currSlotsUsed) {
                abilities[idx] = null;
            }
            currSlotsUsed = new List<int>();
        }
    }
}
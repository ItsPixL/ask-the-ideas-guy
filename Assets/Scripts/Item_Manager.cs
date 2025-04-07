using UnityEngine;
using System.Collections.Generic;
using InteractableManager;

namespace WeaponManager { // create a class for each individual weapon and then create an object of the weapon wherever you need it
    public class Sword : Weapon { 
        private static Sprite sword2D = Resources.Load<Sprite>("2D/Item Sprites/Sword");
        private static GameObject sword3D = Resources.Load<GameObject>("3D/Items/Sword");
        public Sword(List<Ability> abilityList) : base("Sword", sword2D, sword3D, abilityList) {}
    }
}

namespace PowerupManager { // create a class for each individual weapon and then create an object of the weapon wherever you need it
    public class Fire : Powerup { 
        private static Sprite fire2D = Resources.Load<Sprite>("2D/Item Sprites/Sword");
        private static GameObject fire3D = Resources.Load<GameObject>("3D/Items/Sword");
        public Fire(int link) : base("Fire", fire2D, fire3D, link) {}
    }
}
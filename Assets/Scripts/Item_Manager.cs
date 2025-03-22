using UnityEngine;
using System.Collections.Generic;
using InteractableManager;

namespace ItemManager {
    public class Sword: Item {
        private static Sprite sword2D = Resources.Load<Sprite>("2D/Item Sprites/Sword");
        private static GameObject sword3D = Resources.Load<GameObject>("3D/Items/Sword");
        public Sword(List<Ability> abilityList): base("Sword", sword2D, sword3D, abilityList) {}
    }
}
using UnityEngine;
using System.Collections.Generic;
using InteractableManager;
using System;

namespace WeaponManager { // create a class for each individual weapon and then create an object of the weapon wherever you need it
    public class Sword : Weapon { 
        private static Sprite sword2D = Resources.Load<Sprite>("2D/Weapon Sprites/Sword");
        private static GameObject sword3D = Resources.Load<GameObject>("3D/Weapons/Sword");
        public Sword(List<Ability> abilityList) : base("Sword", sword2D, sword3D, abilityList) {}
    }
}

namespace PowerupManager { // create a class for each individual powerup and then create an object of the powerup wherever you need it
    public class Volcanic_Shard : Powerup { 
        private static Sprite volcanic_shard2D = Resources.Load<Sprite>("2D/Powerup Sprites/Volcanic Shard");
        private static GameObject volcanic_shard3D = Resources.Load<GameObject>("3D/Powerups/Volcanic Shard");
        public Volcanic_Shard(int buff) : base("Volcanic Shard", volcanic_shard2D, volcanic_shard3D, buff) {}
    }
    public class Jar_Of_Fireflies : Powerup { 
        private static Sprite jar_of_fireflies2D = Resources.Load<Sprite>("2D/Powerup Sprites/Jar of Fireflies");
        private static GameObject jar_of_fireflies3D = Resources.Load<GameObject>("3D/Powerups/Jar of Fireflies");
        public Jar_Of_Fireflies(int buff) : base("Jar of Fireflies", jar_of_fireflies2D, jar_of_fireflies3D, buff) {}
    }
    public class Void_Tether : Powerup { 
        private static Sprite void_tether2D = Resources.Load<Sprite>("2D/Powerup Sprites/Void Tether");
        private static GameObject void_tether3D = Resources.Load<GameObject>("3D/Powerups/Void Tether");
        public Void_Tether(int buff) : base("Void Tether", void_tether2D, void_tether3D, buff) {}
    }
    public class Frozen_Core : Powerup { 
        private static Sprite frozen_core2D = Resources.Load<Sprite>("2D/Powerup Sprites/Frozen Core");
        private static GameObject frozen_core3D = Resources.Load<GameObject>("3D/Powerups/Frozen Core");
        public Frozen_Core(int buff) : base("Frozen Core", frozen_core2D, frozen_core3D, buff) {}
    }
    public class Poison : Powerup { 
        private static Sprite poison2D = Resources.Load<Sprite>("2D/Powerup Sprites/Poison");
        private static GameObject poison3D = Resources.Load<GameObject>("3D/Powerups/Poison");
        public Poison(int buff) : base("Poison", poison2D, poison3D, buff) {}
    }
}
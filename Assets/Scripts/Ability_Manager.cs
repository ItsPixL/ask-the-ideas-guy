using CooldownManager;
using UnityEngine;
using InteractableManager;
using MonsterManager;
using UIManager;
using Unity.VisualScripting;

namespace AbilityManager {
    public class AbilityManager : MonoBehaviour {
        public UI_Loadout uiLoadout;
        public Loadout loadout;
        int setup = 0;

        void Start() {
            if (Cooldown_Manager.instance == null) Debug.LogError("cooldownManager is null in AbilityManager!");
            Cooldown_Manager.instance.OnCooldownFinished += OnAbilityCooldownFinished;
            Debug.Log("Cooldown Manager initialized and event listener added.");
        }

        private void SetUp()
        {
            UI_Manager uiManager = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
            uiLoadout = uiManager.playerLoadoutUI; // Using the existing UI_Loadout
            Debug.Log("UI_Loadout found in the scene = " + uiLoadout);

            Player_Controller player = GameObject.Find("Player").GetComponent<Player_Controller>();
            loadout = player.playerLoadout;
            Debug.Log("Loadout found in the scene = " + loadout);
        }

        void OnAbilityCooldownFinished(string abilityName)
        {
            Debug.Log($"Ability {abilityName} is off cooldown.");
            if (setup == 0) {
                SetUp();
                setup = 1; // Ensure setup is only done once
            }

            if (abilityName == "JabSword") {
                uiLoadout.enableAbility(2); // or whatever index JabSword uses
            }
            // Add more mappings for other abilities if needed
            
            loadout.resetCooldownLogic(2); // Reset the cooldown logic
        }
    }
    public class Dash : Ability {
        private static Sprite dashSprite = Resources.Load<Sprite>("2D/Ability Sprites/Dash");
        private float dashForce;
        public Dash(int cooldown, float dashForce) : base("Dash", cooldown, dashSprite, index: 0)
        {
            this.dashForce = dashForce;
        }

        public override bool useAbility(GameObject player)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            Vector2 playerDirection = player.GetComponent<Player_Controller>().lastMovementDirection;
            if (playerDirection == new Vector2(0f, 0f))
            {
                playerDirection = new Vector2(0f, 1f);
            }
            playerRb.AddForce(new Vector3(playerDirection.x, 0, playerDirection.y) * dashForce, ForceMode.Impulse);
            return true;
        }
    }
    public class JabSword : Ability
    {
        private static Sprite jabSwordSprite = Resources.Load<Sprite>("2D/Ability Sprites/Dash");
        private float range;
        private float damage;
        private float speed;
        public JabSword(int cooldown, float range, float damage, float speed) : base("JabSword", cooldown, jabSwordSprite, index: 2)
        {
            this.range = range;
            this.damage = damage;
            this.speed = speed;
        }
        public override bool useAbility(GameObject player)
        {
            Debug.Log("Using JabSword ability");
            Vector2 playerDirection = player.GetComponent<Player_Controller>().lastMovementDirection; // getting the last direction so that the game knows where to direct the jab
            Vector3 direction = new Vector3(playerDirection.x, 0, playerDirection.y).normalized;

            // Define jab area in front of player
            Vector3 origin = player.transform.position + direction * (range / 2f);
            Vector3 halfExtents = new Vector3(0.5f, 1f, range / 2f);  // narrow box

            // Detect enemies in jab range
            Collider[] hitEnemies = Physics.OverlapBox(origin, halfExtents, Quaternion.LookRotation(direction), LayerMask.GetMask("Enemy")); // assuming all enemies have the "enemy" mask, need to double check

            foreach (Collider enemy in hitEnemies)
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.takeDamage(damage);
                    Debug.Log($"JabSword hit {monster} for {damage} damage.");
                }
            }

            return true;
        }
        public override void TryUse(GameObject target)
        { // for cooldown purposes
            Debug.Log("Using JabSword ability, " + index);
            string key = nameof(JabSword); // the name of the ability as the key for the dictionary in the cooldown_manager
            if (Cooldown_Manager.instance.CanUseAbility(key))
            { // checking if the ability is off cooldown
                useAbility(target); // Use Ability
                Debug.Log("Jabsword used");
                Cooldown_Manager.instance.UseCooldown(key, cooldown); // Trigger cooldown
            }
            else
            { // if the ability is on cooldown
                float remaining = Cooldown_Manager.instance.GetRemainingCooldown(key);
                Debug.Log($"Jabsword on cooldown: {remaining:F1}s remaining");
            }
            // uiLoadout.enableAbility(index); // Update the UI to show the ability is not in cooldown anymore
        }
    }    
}
using CooldownManager;
using UnityEngine;
using InteractableManager;
using MonsterManager;
using UIManager;

namespace AbilityManager {
    public class Dash: Ability {
        private static Sprite dashSprite = Resources.Load<Sprite>("2D/Ability Sprites/Dash");
        private float dashForce;
        public Dash(int cooldown, float dashForce) : base("Dash", cooldown, dashSprite, index: 0) {
            this.dashForce = dashForce;
        }
        
        public override bool useAbility(GameObject player) {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            Vector2 playerDirection = player.GetComponent<Player_Controller>().lastMovementDirection;
            if (playerDirection == new Vector2(0f, 0f)) {
                playerDirection = new Vector2(0f, 1f);
            }
            playerRb.AddForce(new Vector3(playerDirection.x, 0, playerDirection.y)*dashForce, ForceMode.Impulse);
            return true;
        }
    }
    public class JabSword : Ability
    {
        private Cooldown_Manager cooldownManager;
        private UI_Loadout UI_Loadout;
        private static Sprite jabSwordSprite = Resources.Load<Sprite>("2D/Ability Sprites/Dash");
        private float range;
        private float damage;
        private float speed;
        public JabSword(Cooldown_Manager cooldownManager, int cooldown, float range, float damage, float speed) : base("JabSword", cooldown, jabSwordSprite, index: 1)
        {
            this.range = range;
            this.damage = damage;
            this.speed = speed;
            this.cooldownManager = cooldownManager;
        }
        public override bool useAbility(GameObject player)
        {
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
        public override void TryUse(GameObject target) { // for cooldown purposes
            Debug.Log("Using JabSword ability");
            string key = nameof(JabSword); // the name of the ability as the key for the dictionary in the cooldown_manager
            if (cooldownManager.CanUseAbility(key))
            { // checking if the ability is off cooldown
                useAbility(target); // Use Ability
                Debug.Log("Jabsword used on " + target.name);
                UI_Loadout.enableAbility(index); // Update the UI to show the ability is not in cooldown anymore
                cooldownManager.UseCooldown(key, cooldown); // Trigger cooldown
            }
            else
            { // if the ability is on cooldown
                float remaining = cooldownManager.GetRemainingCooldown(key);
                Debug.Log($"Jabsword on cooldown: {remaining:F1}s remaining");
            }
        }
    }    
}
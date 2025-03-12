using UnityEngine;
using InteractableManager;

namespace AbilityManager {
    public class Dash: Ability {
        private static Sprite dashSprite = Resources.Load<Sprite>("2D/Ability Sprites/Dash");
        private float dashForce;
        public Dash(int cooldown, float dashForce): base("Dash", cooldown, dashSprite) {
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
}
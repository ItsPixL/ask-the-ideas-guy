using UnityEngine;
using MonsterManager;

namespace MonsterManager {
    public class Monster { 
        private float movementSpeed;
        public int sightRange; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private int loseSightRange;
        public GameObject monster; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private GameObject player;
        private bool seenPlayer = false;
        public int fieldOfView; // Change this variable to private once OnDrawGizmos() is no longer needed.

        public Monster(float movementSpeed, int sightRange, int loseSightRange, int fieldOfView) {
            // Constructor function for initialisation.
            this.movementSpeed = movementSpeed;
            this.sightRange = sightRange;
            this.loseSightRange = loseSightRange;
            this.fieldOfView = fieldOfView;
        }

        public void initGameObjects(GameObject monster, GameObject player) {
            // Passes all GameObjects to this class.
            this.monster = monster;
            this.player = player;
        }

        private bool checkFOV(Vector2 playerPos, Vector2 monsterPos) {
            // Checks whether the player is within the monster's field of view.
            Vector2 directionToPlayer = (playerPos-monsterPos).normalized;
            Vector2 monsterForward2D = new Vector2(monster.transform.forward.x, monster.transform.forward.z).normalized;

            float angle = Vector2.Angle(monsterForward2D, directionToPlayer);

            return angle <= fieldOfView / 2;
        }

        private bool reachableDistance(Vector2 playerPos, Vector2 monsterPos, int targetDistance) {
            // Checks whether the player is close enough to the monster.
            float distance = Vector2.Distance(playerPos, monsterPos);
            return distance <= targetDistance;
        }

        private bool detectedPlayer(int targetDistance) {
            // Returns whether the player can be seen or not. 
            Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 monsterPos2D = new Vector2(monster.transform.position.x, monster.transform.position.z);
            if (reachableDistance(playerPos2D, monsterPos2D, sightRange) && checkFOV(playerPos2D, monsterPos2D)) {
                return true;
            }
            return false;
        }

        public void checkForPlayer() {
            if (!seenPlayer && detectedPlayer(sightRange)) {
                seenPlayer = true;
            }
            else if (!detectedPlayer(sightRange)) {
                seenPlayer = false;
            }
            // Uncomment the line below when testing.
            // Debug.Log(seenPlayer);
        }
        
    }
}

public class Monster_Controller : MonoBehaviour {
    Monster entity = new Monster(5f, 10, 15, 160); 

    // The function below is for testing purposes only. It will be removed when all of the code is finalised.
    void OnDrawGizmos() {
        int targetDistance = entity.sightRange;
        if (entity.monster == null) return;

        Vector2 monsterForward2D = new Vector2(entity.monster.transform.forward.x, entity.monster.transform.forward.z).normalized;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -entity.fieldOfView / 2) * monsterForward2D;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, entity.fieldOfView / 2) * monsterForward2D;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(leftBoundary.x, 0, leftBoundary.y) * targetDistance);
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(rightBoundary.x, 0, rightBoundary.y) * targetDistance);
    }

    void Start() {
        entity.initGameObjects(this.gameObject, GameObject.FindWithTag("Player"));
    }

    void Update() {
        entity.checkForPlayer();
    }
}

using UnityEngine;
using MonsterManager;

namespace MonsterManager {
    public class MonsterVision {
        private Transform monsterPos;
        private GameObject player;
        private LineRenderer[] visionLines;
        private Bounds playerBounds;
        public bool linesReset = false;

        public MonsterVision(Transform monsterPos, GameObject player, LineRenderer[] visionLines) {
            this.monsterPos = monsterPos;
            this.visionLines = visionLines;
            this.player = player;
            playerBounds = player.GetComponent<Collider>()?.bounds ?? player.GetComponent<Renderer>()?.bounds ?? new Bounds();
            Debug.Log(playerBounds.extents);
        }

        public void SetUpLines() {
            foreach (var visionLine in visionLines) {
                visionLine.startWidth = 0.05f;
                visionLine.endWidth = 0.05f;
            }
        }

        public void UpdateLines() {
            for (int i=0; i < visionLines.Length; i++) {
                visionLines[i].SetPosition(0, monsterPos.position);
                if (linesReset) {
                    visionLines[i].SetPosition(1, monsterPos.position);
                }
            }
            if (!linesReset) {
                playerBounds = player.GetComponent<Collider>()?.bounds ?? player.GetComponent<Renderer>()?.bounds ?? new Bounds();
                visionLines[0].SetPosition(1, playerBounds.center + new Vector3(0, playerBounds.extents.y, 0));
                visionLines[1].SetPosition(1, playerBounds.center - new Vector3(0, playerBounds.extents.y, 0));
                visionLines[2].SetPosition(1, playerBounds.center - new Vector3(playerBounds.extents.x, 0, 0));
                visionLines[3].SetPosition(1, playerBounds.center + new Vector3(playerBounds.extents.x, 0, 0));
            }
        }
    }
    public class Monster { 
        private float movementSpeed;
        public int sightRange; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private int loseSightRange;
        public GameObject monster; // Change this variable to private once OnDrawGizmos() is no longer needed.
        public GameObject player;
        private bool seenPlayer = false;
        public int fieldOfView; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private MonsterVision vision;

        public Monster(float movementSpeed, int sightRange, int loseSightRange, int fieldOfView) {
            // Constructor function for initialisation.
            this.movementSpeed = movementSpeed;
            this.sightRange = sightRange;
            this.loseSightRange = loseSightRange;
            this.fieldOfView = fieldOfView;
        }

        public void setVisionScript(LineRenderer[] visionLines) {
            vision = new MonsterVision(monster.transform, player, visionLines);
            vision.SetUpLines();
        }

        public void updateVisionLines() {
            vision.UpdateLines();
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
            if (reachableDistance(playerPos2D, monsterPos2D, targetDistance) && checkFOV(playerPos2D, monsterPos2D)) {
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
            // Debug.Log(seenPlayer);
        }
    }
}

public class Monster_Controller : MonoBehaviour {
    private LineRenderer[] visionLines;
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
        visionLines = GetComponentsInChildren<LineRenderer>();
        entity.setVisionScript(visionLines);
        
    }

    void Update() {
        entity.checkForPlayer();
        entity.updateVisionLines();
    }
}

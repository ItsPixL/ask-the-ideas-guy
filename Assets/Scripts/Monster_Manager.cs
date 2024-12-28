using UnityEngine;

namespace MonsterManager {
    public class MonsterVision {
        private Transform monsterPos;
        private GameObject player;
        private LineRenderer[] visionLines;
        private Bounds playerBounds;
        public bool linesReset = true; // this is set to true when the player isn't seen.

        public MonsterVision(Transform monsterPos, GameObject player, LineRenderer[] visionLines) {
            // Constructor function for initialisation.
            this.monsterPos = monsterPos;
            this.visionLines = visionLines;
            this.player = player;
            playerBounds = player.GetComponent<Collider>()?.bounds ?? player.GetComponent<Renderer>()?.bounds ?? new Bounds();
        }

        public void SetUpLines() {
            // Sets up the width for each line (done for hitboxes).
            foreach (var visionLine in visionLines) {
                visionLine.startWidth = 0.05f;
                visionLine.endWidth = 0.05f;
            }
        }

        public bool isClearPath() {
            foreach (var visionLine in visionLines) {
                Vector3 lineStart = visionLine.GetPosition(0);
                Vector3 lineEnd = visionLine.GetPosition(1);
                Vector3 lineDirection = (lineEnd-lineStart).normalized;
                float distance = Vector3.Distance(lineStart, lineEnd);
                if (Physics.Raycast(lineStart, lineDirection, out RaycastHit hitInfo, distance)) {
                    if (hitInfo.collider.CompareTag("Player")) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateLines() {
            // Updates the lines by changing the position of the two ends of each line.
            for (int i=0; i < visionLines.Length; i++) {
                visionLines[i].SetPosition(0, monsterPos.position);
                if (linesReset) {
                    // If the player isn't seen, have the line start and end at the same point to reduce rendering.
                    visionLines[i].SetPosition(1, monsterPos.position);
                }
            }
            if (!linesReset) {
                // If the player is seen, have 4 lines connect to the top, bottom, left and right ends of the player respectively.
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
        private int rotationSpeed;
        public int sightRange; // Change this variable to private once OnDrawGizmos() is no longer needed.
        public GameObject monster; // Change this variable to private once OnDrawGizmos() is no longer needed.
        public GameObject player;
        private bool seenPlayer = false;
        public int fieldOfView; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private MonsterVision vision;
        private Vector2 lastSeenPos = new Vector2(float.NaN, float.NaN);

        public Monster(float movementSpeed, int rotationSpeed, int sightRange, int fieldOfView) {
            // Constructor function for initialisation.
            this.movementSpeed = movementSpeed;
            this.rotationSpeed = rotationSpeed;
            this.sightRange = sightRange;
            this.fieldOfView = fieldOfView;
        }

        public void initGameObjects(GameObject monster, GameObject player) {
            // Passes all GameObjects to this class.
            this.monster = monster;
            this.player = player;
        }

        public void setVisionScript(LineRenderer[] visionLines) {
            // Sets up the vision script for the monster.
            vision = new MonsterVision(monster.transform, player, visionLines);
            vision.SetUpLines();
        }

        public void updateVisionLines() {
            // Updates the lines in the vision script.
            vision.UpdateLines();
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

        private void chasePlayer() {
            Vector2 monsterPos2D = new Vector2(monster.transform.position.x, monster.transform.position.z);
            Vector2 directionToPlayer = (lastSeenPos-monsterPos2D).normalized;
            float angleToPlayer = Vector2.Angle(monsterPos2D, lastSeenPos);
            Debug.Log(angleToPlayer);
            if (directionToPlayer.x != 0) {
                monster.transform.position += new Vector3(directionToPlayer.x*movementSpeed*Time.deltaTime, 0, 0);
            }
            if (directionToPlayer.y != 0) {
                monster.transform.position += new Vector3(0, 0, directionToPlayer.y*movementSpeed*Time.deltaTime);
            }
        }

        public void checkForPlayer() {
            // Detects whether the monster can "see" the player.
            bool isDetectable = detectedPlayer(sightRange);
            vision.linesReset = !isDetectable;
            if (!vision.linesReset) {
                seenPlayer = vision.isClearPath();
                if (seenPlayer) {
                    lastSeenPos = new Vector2(player.transform.position.x, player.transform.position.z);
                }
            }
            else {
                seenPlayer = false;
            }
            if (!float.IsNaN(lastSeenPos.x)) {
                chasePlayer();
            }
            // Debug.Log(seenPlayer);
        }
    }
}
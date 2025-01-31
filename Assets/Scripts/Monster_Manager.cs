using Unity.VisualScripting;
using UnityEngine;

namespace MonsterManager {
    public class Monster { 
        private float movementSpeed;
        private int rotationSpeed;
        public int sightRange; // Change this variable to private once OnDrawGizmos() is no longer needed.
        public int fieldOfView; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private int hearingRange;
        private bool seenPlayer = false;
        public GameObject monster; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private GameObject player;
        private Vector2 lastSeenPos = new Vector2(float.NaN, float.NaN);
        private Vector2 monsterPos2D;
        private Vector2 monsterForward2D;

        public Monster(float movementSpeed, int rotationSpeed, int sightRange, int fieldOfView, int hearingRange) {
            // Constructor function for initialisation.
            this.movementSpeed = movementSpeed;
            this.rotationSpeed = rotationSpeed;
            this.sightRange = sightRange;
            this.fieldOfView = fieldOfView;
            this.hearingRange = hearingRange; 
        }

        public void initGameObjects(GameObject monster, GameObject player) {
            // Passes all GameObjects to this class.
            this.monster = monster;
            this.player = player;
        }

        private bool checkFOV(Vector2 playerPos, Vector2 monsterPos) {
            // Checks whether the player is within the monster's field of view.
            Vector2 directionToPlayer = (playerPos-monsterPos).normalized;
            float angle = Vector2.Angle(monsterForward2D, directionToPlayer);
            return angle <= fieldOfView / 2;
        }

        private bool reachableDistance(Vector2 playerPos, Vector2 monsterPos, int targetDistance) {
            // Checks whether the player is close enough to the monster.
            float distance = Vector2.Distance(playerPos, monsterPos);
            return distance <= targetDistance;
        }

        private bool detectedPlayer(string detectionType) {
            // Checks whether the player can be seen or not. 
            Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);
            if (detectionType == "seen" && reachableDistance(playerPos2D, monsterPos2D, sightRange) && checkFOV(playerPos2D, monsterPos2D)) {
                return true;
            }
            // Checks whether the player can be heard or not. 
            if (detectionType == "heard" && reachableDistance(playerPos2D, monsterPos2D, hearingRange)) {
                return true;
            }
            return false;
        }   

        private void rotateToPos(Vector2 targetPos) {
            // Rotates the monster to see a given position.
            Vector2 directionToPos = (targetPos-monsterPos2D).normalized;
            float angleToPos = Vector2.SignedAngle(monsterForward2D, directionToPos);
            if (angleToPos < rotationSpeed/200) {
                monster.transform.rotation = Quaternion.Euler(0, monster.transform.eulerAngles.y+rotationSpeed*Time.deltaTime, 0);
            }
            else if (angleToPos > rotationSpeed/200) {
                monster.transform.rotation = Quaternion.Euler(0, monster.transform.eulerAngles.y-rotationSpeed*Time.deltaTime, 0);
            }
        }

        private bool isClearPath(Vector3 startPos, Vector3 targetPos, Vector3 extents, GameObject gameObject, bool wantsHit=true) {
            // Checks whether there is a clear line of sight to an object.
            for (int i=0; i<6; i++) {
                Vector3 currEndPos = targetPos;
                Vector3 directionToPos;
                Ray currRay;
                switch (i) {
                    case 0:
                        currEndPos.y += extents.y;
                        break;
                    case 1:
                        currEndPos.y -= extents.y;
                        break;
                    case 2:
                        currEndPos.x += extents.x;
                        break;
                    case 3:
                        currEndPos.x -= extents.x;
                        break;
                    case 4:
                        currEndPos.z += extents.z;
                        break;
                    case 5:
                        currEndPos.z -= extents.z;
                        break;
                }
                directionToPos = (currEndPos-startPos).normalized;
                float currDistance = Vector3.Distance(startPos, currEndPos);
                currRay = new Ray(startPos, directionToPos);
                if (Physics.Raycast(currRay, out RaycastHit hit, currDistance)) {
                    if (!wantsHit) {
                        return false;
                    }
                    if (wantsHit && hit.collider.GameObject() == gameObject) {
                        return true;
                    }
                }
            }
            return !wantsHit;
        }

        private void chasePlayer() {
            // Chases the player down (or the last position that the player was seen in)
            Vector2 directionToPos = (lastSeenPos-monsterPos2D).normalized;
            monster.transform.position += new Vector3(directionToPos.x*movementSpeed*Time.deltaTime, 0, directionToPos.y*movementSpeed*Time.deltaTime);
            rotateToPos(lastSeenPos);
            // when the monster reaches the last position that it saw the player, it can stop moving.
            if (!seenPlayer) {
                float distanceBetweenPos = Vector2.Distance(monsterPos2D, lastSeenPos);
                if (distanceBetweenPos <= movementSpeed/200) {
                    lastSeenPos = new Vector2(float.NaN, float.NaN);
                }   
            }
        }

        public void checkForPlayer() {
            monsterPos2D = new Vector2(monster.transform.position.x, monster.transform.position.z);
            monsterForward2D = new Vector2(monster.transform.forward.x, monster.transform.forward.z).normalized;
            // Checks whether the monster can see or hear the player, to trigger a responsive action.
            bool possiblySeen = detectedPlayer("seen");
            if (possiblySeen) {
                seenPlayer = isClearPath(monster.transform.position, player.transform.position, player.GetComponent<Collider>()?.bounds.extents ?? new Vector3(0, 0, 0), player);
                if (seenPlayer) {
                    lastSeenPos = new Vector2(player.transform.position.x, player.transform.position.z);
                }
            }
            else {
                seenPlayer = false;
            }
            if (!seenPlayer && detectedPlayer("heard")) {
                rotateToPos(new Vector2(player.transform.position.x, player.transform.position.z));
            }

            if (!float.IsNaN(lastSeenPos.x)) {
                chasePlayer();
            }
            // Debug.Log(seenPlayer);
        }
    }
}
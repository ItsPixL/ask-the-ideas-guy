using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using MonsterSpawnerManager;
using System;

namespace MonsterManager {
    // Defines the types of monsters available.
    public enum monsterType {
        Brute
    }

    // Defines all the basic properties and methods of a monster.
    public class Monster { 
        public float health;
        public float damage;
        public MonsterSpawner birthSpawner;
        private float movementSpeed;
        private int rotationSpeed;
        private List<Vector3> dutyPath;
        private float attackRange;
        private float attackCooldown;
        public int sightRange; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private int hearingRange;
        public int fieldOfView; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private bool seenPlayer = false;
        public bool canAttack = true;
        public float lastAttackTime;
        private int dutyIdx = -1;
        private Vector2 currDutyPos;
        private List<Vector2> lastWent = new List<Vector2>();
        private float lastWentRotation;
        public GameObject monster; // Change this variable to private once OnDrawGizmos() is no longer needed.
        private GameObject player;
        private Vector2 lastSeenPos = new Vector2(float.NaN, float.NaN);
        private Vector2 monsterPos2D;
        private Vector2 monsterForward2D;
        private Vector2 playerPos2D;
        private Animator monsterAnimator;

        // Initialises only the very basic monster attributes as a constructor class.
        public Monster(float health, float damage, MonsterSpawner birthSpawner) {
            this.health = health;
            this.damage = damage;
            this.birthSpawner = birthSpawner;
        }

        // Initialises the monster's movement related attributes.
        public void initMovementAttributes(float movementSpeed, int rotationSpeed, List<Vector3> dutyPath) {
            this.movementSpeed = movementSpeed;
            this.rotationSpeed = rotationSpeed;
            this.dutyPath = dutyPath;
        }

        // Initialises the monster's attack related attributes.
        public void initAttackAttributes(float attackRange, float attackCooldown) {
            this.attackRange = attackRange;
            this.attackCooldown = attackCooldown;
        }

        // Initialises the monster's sensory related attributes.
        public void initSensoryAttributes(int sightRange, int hearingRange, int fieldOfView) {
            this.sightRange = sightRange;
            this.hearingRange = hearingRange;
            this.fieldOfView = fieldOfView;
        }

        // Passes all related GameObjects to this class.
        public void initGameObjects(GameObject monster, GameObject player) {
            this.monster = monster;
            this.player = player;
            monsterAnimator = monster.GetComponent<Animator>();
        }

        // Checks whether the player is within the monster's field of view.
        private bool checkFOV(Vector2 playerPos, Vector2 monsterPos) {
            Vector2 directionToPlayer = (playerPos-monsterPos).normalized;
            float angle = Vector2.Angle(monsterForward2D, directionToPlayer);
            return angle <= fieldOfView / 2;
        }

        // Checks whether the player is close enough to the monster.
        private bool reachableDistance(Vector2 playerPos, Vector2 monsterPos, float targetDistance) {
            float distance = Vector2.Distance(playerPos, monsterPos);
            return distance <= targetDistance;
        }

        // Checks whether the monster can detect the player.
        private bool detectedPlayer(string detectionType) {
            // Checks whether the player can be seen or not. 
            if (detectionType == "seen" && reachableDistance(playerPos2D, monsterPos2D, sightRange) && checkFOV(playerPos2D, monsterPos2D)) {
                return true;
            }
            // Checks whether the player can be heard or not. 
            if (detectionType == "heard" && reachableDistance(playerPos2D, monsterPos2D, hearingRange)) {
                return true;
            }
            return false;
        }   

        // Rotates the monster to see a given position (monster only rotates on y-axis).
        private void rotateToPos(Vector2 targetPos) {
            Vector3 directionToPos = (new Vector3(targetPos.x, 0, targetPos.y)-monster.transform.position).normalized;
            directionToPos.y = 0;
            // In case the monster is already exactly on its target position.
            if (directionToPos.sqrMagnitude < 0.001f)
                return;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPos);
            float angleDifference = Quaternion.Angle(monster.transform.rotation, targetRotation);
            if (angleDifference > rotationSpeed*Time.deltaTime) {
                monster.transform.rotation = Quaternion.RotateTowards(monster.transform.rotation, targetRotation, 
                rotationSpeed * Time.deltaTime);
            }
        }

        // Checks whether there is a clear line of sight (or not, when wantsHit=False) to an object or location.
        private bool isClearPath(Vector3 startPos, Vector3 targetPos, Vector3 extents, GameObject gameObject=null, bool wantsHit=true) {
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

        // Chases the player down (or the last position that the player was seen in).
        private void chasePlayer() {
            Vector2 directionToPos = (lastSeenPos-monsterPos2D).normalized;
            monster.transform.position += new Vector3(directionToPos.x*movementSpeed*Time.deltaTime, 0, directionToPos.y*movementSpeed*Time.deltaTime);
            rotateToPos(lastSeenPos);
            if (Math.Abs(monster.transform.eulerAngles.y-lastWentRotation) >= 5 && monsterPos2D != lastWent[lastWent.Count-1]) {
                lastWent.Add(monsterPos2D);
                lastWentRotation = monster.transform.eulerAngles.y;
            }
            // When the monster reaches the last position that it saw the player, it can stop moving.
            if (!seenPlayer) {
                float distanceBetweenPos = Vector2.Distance(monsterPos2D, lastSeenPos);
                if (distanceBetweenPos <= movementSpeed/200) {
                    lastSeenPos = new Vector2(float.NaN, float.NaN);
                }   
            }
        }

        // Checks whether the monster can see or hear the player, to trigger a responsive action.
        public void checkForPlayer() {
            monsterPos2D = new Vector2(monster.transform.position.x, monster.transform.position.z);
            monsterForward2D = new Vector2(monster.transform.forward.x, monster.transform.forward.z).normalized;
            playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);
            bool possiblySeen = detectedPlayer("seen");
            bool prevSeen = seenPlayer;
            if (possiblySeen) {
                seenPlayer = isClearPath(monster.transform.position, player.transform.position, player.GetComponent<Collider>()?.bounds.extents ?? new Vector3(0, 0, 0), player);
                if (seenPlayer) {
                    if (!prevSeen) {
                        lastWent.Add(monsterPos2D);
                        lastWentRotation = monster.transform.eulerAngles.y;
                    }
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
            else if (!detectedPlayer("heard")) {
                checkOnDuty();
            }
        }

        // Finds the closest duty position from the monster to start patrolling from.
        public int findDutyPos() {
            float minDistance = float.MaxValue;
            int currDutyIdx = 0;
            for (int i=0; i < dutyPath.Count; i++) {
                Vector3 vertex = dutyPath[i];
                bool isVertexUnobstructed = isClearPath(monster.transform.position, vertex, monster.GetComponent<Collider>()?.bounds.extents ?? new Vector3(), wantsHit:false);
                if (isVertexUnobstructed) {
                    Vector2 vertex2D = new Vector2(vertex.x, vertex.z);
                    float currDistance = Vector2.Distance(monsterPos2D, vertex2D);
                    if (currDistance < minDistance) {
                        minDistance = currDistance;
                        currDutyIdx = i;
                    }
                }
            }
            return currDutyIdx;
        }

        // Checks if the monster is back within duty path despite not going back to the exact same spot.
        public bool checkIfRetreated() {
            int possibleDutyIdx = findDutyPos();
            if (possibleDutyIdx == -1) {
                return false;
            }
            dutyIdx = possibleDutyIdx;
            currDutyPos = new Vector2(dutyPath[dutyIdx].x, dutyPath[dutyIdx].z);
            return true;
        }

        // Travels on a fixed duty path, and retreats back to the duty path after losing track of the player.
        public void checkOnDuty() {
             if (dutyIdx < 0) {
                dutyIdx = findDutyPos();
                currDutyPos = new Vector2(dutyPath[dutyIdx].x, dutyPath[dutyIdx].z);
            }
            Vector2 directionToPos;
            Vector2 nextDutyPos;
            if (lastWent.Count > 0) {
                nextDutyPos = lastWent[lastWent.Count-1];
            }
            else {
                nextDutyPos = currDutyPos;
            }
            float distanceBetweenPos = Vector2.Distance(monsterPos2D, nextDutyPos);
            if (distanceBetweenPos <= movementSpeed/200) {
                if (lastWent.Count > 0) {
                    lastWent.RemoveAt(lastWent.Count-1);
                }
                else {
                    dutyIdx += 1;
                    if (dutyIdx == dutyPath.Count) {
                        dutyIdx = 0;
                    }
                    currDutyPos = new Vector2(dutyPath[dutyIdx].x, dutyPath[dutyIdx].z);
                }
            }
            else if (lastWent.Count > 0 && checkIfRetreated()) {
                lastWent = new List<Vector2>();
            }
            directionToPos = (nextDutyPos-monsterPos2D).normalized;
            monster.transform.position += new Vector3(directionToPos.x*movementSpeed/2*Time.deltaTime, 0, directionToPos.y*movementSpeed/2*Time.deltaTime);
            rotateToPos(nextDutyPos);
        }

        // Checks if the monster can attack the player.
        public virtual void checkForAttack() {
            if (seenPlayer && reachableDistance(playerPos2D, monsterPos2D, attackRange) && canAttack) {
                dealDamage(damage);
                lastAttackTime = Time.time;
                canAttack = false;
            }
            if (Time.time-lastAttackTime >= attackCooldown) {
                canAttack = true;
            }
        }

        public void takeDamage(float damageAmount)
        {
            health -= damageAmount;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Enemy has died");
        }

        // When the damage is actually dealt - This function can be activated through animation or other code.
        public void dealDamage(float damage)
        {
            player.GetComponent<Player_Controller>().playerHealth -= damage;
        }
    }

    public class Brute: Monster {
        public Brute(float health, float damage, MonsterSpawner birthSpawner): base(health, damage, birthSpawner) {}
    }
}
using UnityEngine;
using MonsterManager;
using UnityEngine.Rendering;

public class Monster_Controller : MonoBehaviour {
    // Plan is to get rid of ALL of these variables from the inspector, only there currently for testing purposes.
    public float health;
    public float damage;
    public float movementSpeed;
    public int rotationSpeed;
    public float attackRange;
    public int sightRange;
    public int hearingRange;
    public int fieldOfView;
    private Monster entity;
    private float lastAttackTime;
    public enum monsterType {
        Brute
    }
    public monsterType currMonsterType;

    void Start() {
        entity = new Monster(health, damage);
        entity.initMovementAttributes(movementSpeed, rotationSpeed);
        entity.initAttackAttributes(attackRange);
        entity.initSensoryAttributes(sightRange, hearingRange, fieldOfView);
        entity.initGameObjects(gameObject, GameObject.FindWithTag("Player")); 
    }

    void Update() {
        entity.checkForPlayer();
        if (!entity.isAttacking) {
            entity.checkForAttack();
            lastAttackTime = Time.time;
        }
        else if (Time.time-lastAttackTime > 1.5) {
            entity.isAttacking = false;
        }
    }

    // Initialises the specific script for that specific monster (this script is the script that all monsters have attached).
    public void initSpecificScript(string inputtedMonsterType) {
        monsterType currMonsterType = monsterType.Brute;
        if (System.Enum.TryParse(inputtedMonsterType, out monsterType outputtedMonsterType)) {
            currMonsterType = outputtedMonsterType;
        }
        switch (currMonsterType) {
            case monsterType.Brute:
                gameObject.AddComponent<Brute_Controller>();
                break;
        }
        this.currMonsterType = currMonsterType;
    }

    // The function below is for testing purposes only. It will be removed when all of the code is finalised.
    void OnDrawGizmos() {
        if (entity == null) return;

        int targetDistance = entity.sightRange;

        Vector2 monsterForward2D = new Vector2(entity.monster.transform.forward.x, entity.monster.transform.forward.z).normalized;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -entity.fieldOfView / 2) * monsterForward2D;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, entity.fieldOfView / 2) * monsterForward2D;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(leftBoundary.x, 0, leftBoundary.y) * targetDistance);
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(rightBoundary.x, 0, rightBoundary.y) * targetDistance);
    }
}

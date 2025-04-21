using UnityEngine;
using MonsterManager;

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
    public monsterType currMonsterType = monsterType.Brute;
    private MonsterComponent newComponent;

    void Start() {

    }

    void Update() {

    }

    // Initialises the specific script for that specific monster (this script is the script that all monsters have attached).
    public void initSpecificScript(monsterType inputtedMonsterType) {
        currMonsterType = inputtedMonsterType;
        switch (currMonsterType) {
            case monsterType.Brute:
                gameObject.AddComponent<Brute_Controller>();
                newComponent = gameObject.GetComponent<Brute_Controller>();
                break;
        }
    }

    // Initialises the new (specific) monster, including health and damage.
    public void initMonster(float health, float damage) {
        newComponent.initMonster(health, damage);
    }

    // Initialises the movement related attributes of the specific monster.
    public void initMonsterMovement(float movementSpeed, int rotationSpeed) {
        newComponent.initMonsterMovement(movementSpeed, rotationSpeed);
    }

    // Initialises the attack related attributes of the specific monster.
    public void initMonsterAttack(float attackRange, float attackCooldown) {
        newComponent.initMonsterAttack(attackRange, attackCooldown);
    }

    // Initialises the sensory related attributes of the specific monster.
    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView) {
        newComponent.initMonsterSenses(sightRange, hearingRange, fieldOfView);
    }

    // Used to activate/deactivate the monster.
    public void setMonsterStatus(bool monsterStatus) {
        newComponent.setMonsterStatus(monsterStatus);
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

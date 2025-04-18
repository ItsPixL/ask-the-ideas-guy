using UnityEngine;
using MonsterManager;
using UnityEngine.Rendering;

public class Monster_Controller : MonoBehaviour {
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

    void Start() {
        entity = new Monster(health, damage, movementSpeed, rotationSpeed, attackRange, sightRange, hearingRange, fieldOfView);
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

using UnityEngine;
using MonsterManager;

public class Brute_Controller: MonoBehaviour, MonsterComponent {
    public Brute brute;
    private bool isMonsterActive = false;
    void Start() {
        
    }

    void Update() {
        if (isMonsterActive) {
            brute.checkForPlayer();
            brute.checkForAttack();
        }
    }

    public void initMonster(float health, float damage) {
        brute = new Brute(health, damage);
        brute.initGameObjects(gameObject, GameObject.FindWithTag("Player"));
    }

    public void initMonsterMovement(float movementSpeed, int rotationSpeed) {
        brute.initMovementAttributes(movementSpeed, rotationSpeed);
    }

    public void initMonsterAttack(float attackRange, float attackCooldown) {
        brute.initAttackAttributes(attackRange, attackCooldown);
    }

    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView) {
        brute.initSensoryAttributes(sightRange, hearingRange, fieldOfView);
    }

    public void setMonsterStatus(bool monsterStatus) {
        isMonsterActive = monsterStatus;
    }
 
}
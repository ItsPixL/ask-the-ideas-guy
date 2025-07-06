using UnityEngine;
using MonsterManager;
using MonsterSpawnerManager;
using System.Collections.Generic;

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

    public void initMonster(float health, float damage, MonsterSpawner birthSpawner) {
        brute = new Brute(health, damage, birthSpawner);
        brute.initGameObjects(gameObject, GameObject.FindWithTag("Player"));
    }

    public void initMonsterMovement(float movementSpeed, int rotationSpeed, List<Vector3> dutyPath) {
        brute.initMovementAttributes(movementSpeed, rotationSpeed, dutyPath);
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
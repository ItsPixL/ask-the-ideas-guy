using UnityEngine;
using MonsterManager;
using MonsterSpawnerManager;
using System.Collections.Generic;

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
    public void initMonster(float health, float damage, MonsterSpawner birthSpawner) {
        newComponent.initMonster(health, damage, birthSpawner);
    }

    // Initialises the movement related attributes of the specific monster.
    public void initMonsterMovement(float movementSpeed, int rotationSpeed, List<Vector3> dutyPath) {
        newComponent.initMonsterMovement(movementSpeed, rotationSpeed, dutyPath);
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
}

using UnityEngine;
using MonsterManager;

public class Brute_Controller: MonoBehaviour {
    public Brute brute;
    void Start() {
        
    }

    void Update() {
        
    }

    public void initMonster(float health, float damage) {
        brute = new Brute(health, damage);
    }

    public void initMonsterMovement(float movementSpeed, int rotationSpeed) {
        brute.initMovementAttributes(movementSpeed, rotationSpeed);
    }

    public void initMonsterAttack(float attackRange) {
        brute.initAttackAttributes(attackRange);
    }

    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView) {
        brute.initSensoryAttributes(sightRange, hearingRange, fieldOfView);
    }
 
}
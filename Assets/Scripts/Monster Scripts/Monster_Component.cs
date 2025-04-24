using UnityEngine;
using System.Collections.Generic;
using MonsterSpawnerManager;

public interface MonsterComponent {
    public void initMonster(float health, float damage, MonsterSpawner birthSpawner) {}
    public void initMonsterMovement(float movementSpeed, int rotationSpeed, List<Vector3> dutyPath) {}
    public void initMonsterAttack(float attackRange, float attackCooldown) {}
    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView) {}
    public void setMonsterStatus(bool monsterStatus) {}
}
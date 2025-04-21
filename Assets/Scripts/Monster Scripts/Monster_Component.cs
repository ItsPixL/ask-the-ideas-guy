public interface MonsterComponent {
    public void initMonster(float health, float damage) {}
    public void initMonsterMovement(float movementSpeed, int rotationSpeed) {}
    public void initMonsterAttack(float attackRange, float attackCooldown) {}
    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView) {}
    public void setMonsterStatus(bool monsterStatus) {}
}
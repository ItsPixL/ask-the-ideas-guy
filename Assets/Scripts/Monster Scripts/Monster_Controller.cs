using UnityEngine;
using MonsterManager;
using MonsterSpawnerManager;
using System.Collections.Generic;

public class Monster_Controller : MonoBehaviour
{
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
    public Monster monster;
    public MonsterComponent newComponent;
    private bool isMonsterActive = false;

    void Start()
    {

    }

    void Update()
    {
        if (isMonsterActive)
        {
            monster.checkForPlayer();
            monster.checkForAttack();
            // ONLY FOR TESTING PURPOSES! THESE VARIABLES WON'T EXIST IN THE ACTUAL GAME //
            health = monster.health;
            damage = monster.damage;
            movementSpeed = monster.movementSpeed;
            rotationSpeed = monster.rotationSpeed;
            attackRange = monster.attackRange;
            sightRange = monster.sightRange;
            hearingRange = monster.hearingRange;
            fieldOfView = monster.fieldOfView;
        }
    }

    // Initialises the specific script for that specific monster (this script is the script that all monsters have attached).
    // Also initialises health, damange and birthspawner.
    public void initBasicMonster(monsterType inputtedMonsterType, float health, float damage, MonsterSpawner birthSpawner)
    {
        currMonsterType = inputtedMonsterType;
        switch (currMonsterType)
        {
            case monsterType.Brute:
                monster = new Brute(health, damage, birthSpawner);
                monster.initGameObjects(gameObject, GameObject.FindWithTag("Player"));
                gameObject.AddComponent<Brute_Controller>();
                newComponent = gameObject.GetComponent<Brute_Controller>();
                break;
        }
    }

    // Initialises the movement related attributes of the specific monster.
    public void initMonsterMovement(float movementSpeed, int rotationSpeed, List<Vector3> dutyPath)
    {
        monster.initMovementAttributes(movementSpeed, rotationSpeed, dutyPath);
    }

    // Initialises the attack related attributes of the specific monster.
    public void initMonsterAttack(float attackRange, float attackCooldown)
    {
        monster.initAttackAttributes(attackRange, attackCooldown);
    }

    // Initialises the sensory related attributes of the specific monster.
    public void initMonsterSenses(int sightRange, int hearingRange, int fieldOfView)
    {
        monster.initSensoryAttributes(sightRange, hearingRange, fieldOfView);
    }

    // Used to activate/deactivate the monster.
    public void setMonsterStatus(bool monsterStatus)
    {
        isMonsterActive = monsterStatus;
    }

    /* Very important note Adi...I never assign the variables in the component (seen above) like "health" and "damage".
    That's because I pass all the information down to the monster itself.
    So the reason that the code wasn't working was that you needed to target "monster.health" and "monster.damage"
    Since the information is used there.
    Also I did reform my code to be slightly simpler. Hopefully this small fix means no hustle for you.
    You will notice that I assign and update the varaibles above, but that is just for testing so that you can see the changes live. */
    public void takeDamage(float damageAmount)
    {
        Debug.Log($"health is now {monster.health}");
        monster.health -= damageAmount;
        Debug.Log($"health is now {monster.health}");
        if (monster.health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died");
        Destroy(gameObject);
    }
}

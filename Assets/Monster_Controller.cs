using UnityEngine;
using System;

public class Monster {
    private float movementSpeed = 5f;
    private float sightRange = 10f;
    private float loseSightRange = 15f;
    public GameObject monster;
    public GameObject player;
    private bool seenPlayer = false;
    private int fieldOfView = 160;

    bool checkFOV() {
        Vector3 directionToPlayer = (player.transform.position - monster.transform.position).normalized;

        float angle = Vector3.Angle(monster.transform.forward, directionToPlayer);

        return angle <= fieldOfView / 2;
    }

    bool reachableDistance(int targetDistance) {
        float distance = Vector3.Distance(player.transform.position, monster.transform.position);
        return distance <= targetDistance;
    }

    void OnDrawGizmos(int targetDistance)
{
        if (monster == null) return;

        // Draw the FOV cone
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * monster.transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * monster.transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(monster.transform.position, leftBoundary * targetDistance);
        Gizmos.DrawRay(monster.transform.position, rightBoundary * targetDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monster.transform.position, targetDistance);
}

    void checkForPlayer() {
        // if (!seenPlayer && reachableDistance(sightRange))
    }
      
}
public class Monster_Controller : MonoBehaviour {
    void Start() {

    }

    void Update() {

    }
}

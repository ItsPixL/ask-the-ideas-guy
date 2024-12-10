using UnityEngine;
using System;

public class Monster {
    private float movementSpeed = 5f;
    private float sightRange = 10f;
    private float loseSightRange = 15f;
    public Rigidbody monsterRb;
    public Transform monster;
    public Transform player;
    private bool seenPlayer = false;

    float calculateDistance() {
        float xDistance = monster.position.x-player.position.x;
        float zDistance = monster.position.z-player.position.z;
        return (float)Math.Sqrt((float)Math.Pow(xDistance, 2)+(float)Math.Pow(zDistance, 2));
    }

    void checkForPlayer() {
        // if (!seenPlayer && )
    }
      
}
public class Monster_Controller : MonoBehaviour {
    void Start() {

    }

    void Update() {

    }
}

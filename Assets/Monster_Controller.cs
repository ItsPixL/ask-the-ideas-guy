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

    bool reachableDistance(int distance) {
        float xDistance = monster.transform.position.x-player.transform.position.x;
        float zDistance = monster.transform.position.z-player.transform.position.z;
        return Math.Pow(xDistance, 2)+Math.Pow(zDistance, 2) <= Math.Pow(distance, 2);
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

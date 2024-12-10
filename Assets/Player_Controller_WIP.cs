using UnityEngine;

public class Player_Controller_WIP : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 5f;
    private bool allowPlayerInput = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start() {
        // Nothing here yet!
    }

    void MovePlayer(float forceX, float forceY, float forceZ) {
        // Moves the character.
        rb.AddForce(forceX * Time.deltaTime, forceY * Time.deltaTime, forceZ * Time.deltaTime, ForceMode.VelocityChange);
    }

    void InitPlayerMovement() {
        // Allows character movement by player input.
        if (Input.GetKey("w")) { // forwards
            MovePlayer(0f, 0f, force);
        }
        if (Input.GetKey("s")) { // backwards
            MovePlayer(0f, 0f, -force);
        }
        if (Input.GetKey("a")) { // left
            MovePlayer(-force, 0f, 0f);
        }
        if (Input.GetKey("d")) { // right
            MovePlayer(force, 0f, 0f);
        }
    }

    void FixedUpdate() {
        if (allowPlayerInput) {
            // All player input functions should be put here.
            InitPlayerMovement();
        }
    }
}

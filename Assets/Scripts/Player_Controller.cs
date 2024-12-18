using UnityEngine;
using InventoryManager;

public class Player_Controller : MonoBehaviour {
    public Rigidbody rb;
    public float force = 5f;
    private bool allowPlayerInput = true;
    private Inventory playerInventory = new Inventory(5);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerInventory.resetInventory();
    }

    void MovePlayer(float forceX, float forceY, float forceZ) {
        // Moves the character.
        rb.AddForce(forceX * Time.deltaTime, forceY * Time.deltaTime, forceZ * Time.deltaTime, ForceMode.VelocityChange);
    }

    void InitPlayerMovement() {
        // Allows character movement by player input.
        if (Input.GetKey("w")) { // forwardss
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

    // Updates inventory information to respond to UI interaction.
    public void updateInventoryStatus(int targetIdx) {
        playerInventory.fetchCurrItem(targetIdx);
    }

    // Update function used for all physics updates.
    void FixedUpdate() {
        if (allowPlayerInput) {
            // All physics related player input functions should be put here.
            InitPlayerMovement();
        }
    }

    // All other updates are in the standard Update() function.
    void Update() {
        if (allowPlayerInput) {
            // All other player input functions should be put here.
            playerInventory.navigateInventory();
        }
    }
}

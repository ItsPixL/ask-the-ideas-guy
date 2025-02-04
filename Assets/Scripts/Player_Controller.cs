using UnityEngine;
using InteractableManager;
using UIManager;
using System.Collections.Generic;

public class Player_Controller : MonoBehaviour {
    public Rigidbody playerRb;
    public float force = 5f;
    public int playerHealth = 100;
    public int playerEnergy = 100;
    private bool allowPlayerInput = true;
    private Inventory playerInventory;
    private Loadout playerLoadout;
    private UI_Manager UI_Controller;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        UI_Controller = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
        playerInventory = new Inventory(UI_Controller.inventoryButtons.Count, new List<int>{6, 7, 8, 9, 0});
        playerInventory.resetInventory();
    }

    // Moves the character.
    void MovePlayer(float forceX, float forceY, float forceZ) {
        playerRb.AddForce(forceX * Time.deltaTime, forceY * Time.deltaTime, forceZ * Time.deltaTime, ForceMode.VelocityChange);
    }

    // Allows character movement by player input.
    void InitPlayerMovement() {
        if (Input.GetKey("w")) { 
            MovePlayer(0f, 0f, force);
        }
        if (Input.GetKey("s")) { 
            MovePlayer(0f, 0f, -force);
        }
        if (Input.GetKey("a")) { 
            MovePlayer(-force, 0f, 0f);
        }
        if (Input.GetKey("d")) { 
            MovePlayer(force, 0f, 0f);
        }
    }

    // Updates inventory information to respond to UI interaction.
    public void updateInventoryStatus(int targetIdx) {
        playerInventory.fetchCurrItem(targetIdx);
        UI_Controller.updateInventoryStatusUI(targetIdx);
    }

    public void checkLoadoutInteraction(int targetIdx) {
        if (playerLoadout.canUseAbility(targetIdx, playerEnergy)) {
            playerEnergy = playerLoadout.useAbility(targetIdx, playerEnergy);
        }
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
            bool wasInventoryInput = playerInventory.checkKeyInput();
            int checkLoadoutInput = playerLoadout.checkKeyInput();
            if (wasInventoryInput) {
                if (playerInventory.selectedSlot) {
                    updateInventoryStatus(playerInventory.currIdx);
                }
                else {
                    updateInventoryStatus(-1);
                }
            }
            if (checkLoadoutInput > -1) {
                checkLoadoutInteraction(checkLoadoutInput);
            }
        }
    }
}

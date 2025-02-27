using UnityEngine;
using InteractableManager;
using UIManager;
using System.Collections.Generic;
using System.Threading;

public class Player_Controller : MonoBehaviour {
    private Rigidbody playerRb;
    public float playerForce = 5f;
    public float maxHealth = 100f;
    public float playerHealth;
    public float maxEnergy = 100f;
    public float playerEnergy;
    private bool allowPlayerInput = true;
    private Inventory playerInventory;
    private Loadout playerLoadout;
    private UI_Manager UI_Controller;
    int milliseconds = 3000;

    private string gradientMovement = "backwards";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerHealth = maxHealth;
        playerEnergy = maxEnergy;
        UI_Controller = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
        playerInventory = new Inventory(UI_Controller.inventoryButtons.Count, new List<int>{6, 7, 8, 9, 0});
        playerLoadout = new Loadout(UI_Controller.loadoutButtons.Count, new List<int>{1, 2, 3, 4});
        playerInventory.resetInventory();
        playerLoadout.resetLoadout();
        allowPlayerInput = true;
        UI_Controller.setUpMetricBars(maxHealth, maxEnergy);
    }

    // Moves the character.
    void MovePlayer(float forceX, float forceY, float forceZ) {
        playerRb.AddForce(forceX * Time.deltaTime, forceY * Time.deltaTime, forceZ * Time.deltaTime, ForceMode.VelocityChange);
    }

    // Allows character movement by player input.
    void InitPlayerMovement() {
        if (Input.GetKey("w")) { 
            MovePlayer(0f, 0f, playerForce);
        }
        if (Input.GetKey("s")) { 
            MovePlayer(0f, 0f, -playerForce);
        }
        if (Input.GetKey("a")) { 
            MovePlayer(-playerForce, 0f, 0f);
        }
        if (Input.GetKey("d")) { 
            MovePlayer(playerForce, 0f, 0f);
        }
    }

    private void PlayerDied() {
        UI_Manager.GameOver();
        gameObject.SetActive(false); // not sure why this is needed
    }

    // Updates inventory information and UI to respond to player interaction.
    public void updateInventoryStatus(int targetIdx) {
        playerInventory.fetchCurrItem(targetIdx);
        UI_Controller.updateInventoryStatusUI(targetIdx);
    }

    // Updates loadout information and UI to respond to player interaction. 
    public void updateLoadoutStatus(int targetIdx) {
        if (playerLoadout.canUseAbility(targetIdx, playerEnergy)) {
            playerEnergy = playerLoadout.useAbility(targetIdx, playerEnergy);
            UI_Controller.updateLoadoutStatusUI(targetIdx);
        } 
    }

    // Update function used for all physics updates.
    void FixedUpdate() {
        if (allowPlayerInput) {
            InitPlayerMovement();
        }
    }

    // All other updates are in the standard Update() function, such as checking for other player inputs.
    void Update() {
        if (allowPlayerInput) {
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
                updateLoadoutStatus(checkLoadoutInput);
            }
        }
        UI_Controller.updateMetricBars(playerHealth, playerEnergy);
        if (playerHealth <= 0) {
            allowPlayerInput = false;
            Thread.Sleep(milliseconds);
            PlayerDied();
        }
        playerHealth -= 0.5f;
    }
}

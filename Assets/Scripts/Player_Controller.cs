using UnityEngine;
using InteractableManager;
using UIManager;
using System.Collections.Generic;
using WeaponManager;
using PowerupManager;
using AbilityManager;
// using System.Threading;
using TMPro;

public class Player_Controller : MonoBehaviour
{
    public float playerForce = 5f;
    public float acceleration = 50f; // Higher = snappier, lower = smoother
    public float maxHealth = 100f;
    public float playerHealth;
    public float pickUpRange = 3f;
    private bool allowPlayerInput = true;
    private bool isPlayerDead = false;
    private Vector2 lastPos2D;
    private Rigidbody playerRb;
    public Inventory playerWeaponInventory;
    public PowerupInventory playerPowerupInventory;
    public Loadout playerLoadout;
    private UI_Manager UI_Controller;
    [HideInInspector] public Vector2 lastMovementDirection;
    private bool tested = false;
    private Animator playerAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        playerHealth = maxHealth;
        UI_Controller = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
        playerWeaponInventory = new Inventory(UI_Controller.weaponInventoryButtons.Count, new List<int> { 7, 8, 9, 0 });
        playerWeaponInventory.resetInventory();
        playerPowerupInventory = new PowerupInventory(UI_Controller.powerupInventoryButtons.Count, new List<char> { 'c', 'v', 'b', 'n', 'm'});
        playerPowerupInventory.resetInventory();
        playerLoadout = new Loadout(UI_Controller.loadoutButtons.Count, new List<int> { 1, 2, 3, 4 });
        playerLoadout.resetLoadout();
        UI_Controller.setUpMetricBars(maxHealth);
        UI_Controller.SetPlayerPowerupInventory(playerPowerupInventory);
        Ability permaDashAbility = new Dash(0.5f, 10);
        playerLoadout.addAbility(permaDashAbility, false);
        UI_Controller.updateAbilityIcon(0, permaDashAbility.icon, 255);
    }

    // Update function used for all physics updates.
    void FixedUpdate() {
        if (allowPlayerInput) {
            Vector3 moveDir = InitPlayerMovement();
            if (moveDir != Vector3.zero) {
                Look(moveDir);
            }
        }
    }

    // All other updates are in the standard Update() function, such as checking for other player inputs.
    void Update() {
        Vector2 playerPos2D = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        if (playerPos2D != lastPos2D) {
            lastMovementDirection = (playerPos2D-lastPos2D).normalized;
        }
        lastPos2D = playerPos2D;
        if (allowPlayerInput) {
            int weaponInventoryInput = playerWeaponInventory.checkKeyInput();
            int powerupInventoryInput = playerPowerupInventory.checkKeyInput();
            int loadoutInput = playerLoadout.checkKeyInput();
            if (weaponInventoryInput != -1) {
                updateWeaponInventoryStatusSecure(weaponInventoryInput);
                updateAbilities(playerWeaponInventory.currIdx, playerWeaponInventory.selectedSlot);
            }
            if (powerupInventoryInput != -1) {
                updatePowerupInventoryStatusSecure(powerupInventoryInput);
            }
            if (loadoutInput > -1) {
                updateLoadoutStatus(loadoutInput);
            }
            UI_Controller.updateMetricBars(playerHealth);
            checkForDrop(KeyCode.E, playerWeaponInventory);
            checkForDrop(KeyCode.R, playerPowerupInventory);
        }
        UI_Controller.updateMetricBars(playerHealth);
        if (playerHealth <= 0 && !isPlayerDead) {
            allowPlayerInput = false;
            isPlayerDead = true;
            Debug.Log("Player has died.");
            PlayerDied();
        }
        // playerHealth -= 0.25f;
        // Debug.Log("Player Health: " + playerHealth);
    }

    // Moves the character.
    public void MovePlayer(Vector3 velocity) {
        velocity.y = 0f; // always keeping the Y value zero to prevent vertical movement
        if (velocity == Vector3.zero)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero; // Stop any rotation
            playerAnimator.SetBool("Run", false);
            playerAnimator.SetBool("Idle", true);
        }
        else
        {
            Vector3 newVelocity = Vector3.MoveTowards(playerRb.linearVelocity, velocity, acceleration * Time.fixedDeltaTime);
            newVelocity.y = 0f;
            playerRb.linearVelocity = newVelocity;
            playerAnimator.SetBool("Run", true);
            playerAnimator.SetBool("Idle", false);
        }
    }

    // Allows character movement by player input.
    public Vector3 InitPlayerMovement() { // stop the vid at 7:20
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey("w")) {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey("s")) {
            moveDir += Vector3.back;
        }
        if (Input.GetKey("a"))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey("d"))
        {
            moveDir += Vector3.right;
        }

        moveDir = moveDir.normalized;
        Vector3 velocity = moveDir * playerForce; // playerForce is now your speed
        MovePlayer(velocity);
        return moveDir;
    }

    void Look(Vector3 direction){
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rot;
    }

    // Checks if the keybind ('E'/'R') has been pressed, and drops the item that is being held.
    public void checkForDrop(KeyCode keybind, Inventory inventory) {
        if (Input.GetKeyDown(keybind)) {
            Item currItem = inventory.items[inventory.currIdx];
            if (inventory.selectedSlot && currItem is not null) {
                removeItemFromInventory(inventory.currIdx, inventory);
                float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
                currItem.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
                Quaternion.Euler(0, playerRotationY, 0));
            }
        }
    }

    private void PlayerDied() {
        UI_Controller.GameOver();
        gameObject.SetActive(false);
    }

    // Adds an item to the correct inventory and updates UI to include the new item icons.
    public void addItemToInventory(Item item, Inventory inventory) {
        Item prevItem = inventory.items[inventory.currIdx];
        if (prevItem is not null && !inventory.spaceInInventory()) {
            if (item is Weapon) {
                removeItemFromInventory(inventory.currIdx, playerWeaponInventory); 
            }
            else if (item is Powerup) {
                removeItemFromInventory(inventory.currIdx, playerPowerupInventory); 
            }
            float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            prevItem.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
            Quaternion.Euler(0, playerRotationY, 0));
        }
        inventory.addItem(item);
        if (item is Weapon) {
            updateInventoryStatus(inventory.currIdx, inventory, 'W');
            UI_Controller.updateInventoryIcon(UI_Controller.weaponInventoryButtons, playerWeaponInventory.currIdx, item.object2D, 255);
        }
        else if (item is Powerup) {
            updateInventoryStatus(inventory.currIdx, inventory, 'P');
            UI_Controller.updateInventoryIcon(UI_Controller.powerupInventoryButtons, playerPowerupInventory.currIdx, item.object2D, 255);
        }
        if (item is Weapon) {
            updateAbilities(inventory.currIdx, inventory.selectedSlot);
        }
        else if (item is Powerup) {
            // Add the stat buffs to the weapon
        }
    }

    // Adds all abilities associated with a weapon to the loadout and updates UI to include the new ability icons.
    public void addAbilitiesToLoadout(Weapon weapon) {
        foreach (Ability ability in weapon.abilityList) {
            int abilityIdx = playerLoadout.addAbility(ability, true);
            UI_Controller.updateAbilityIcon(abilityIdx, ability.icon, 255);
        }
    }

    // Removes an item from its respective inventory and updates UI to remove those item icons.
    public void removeItemFromInventory(int targetIdx, Inventory inventory) {
        inventory.removeItem(targetIdx);
        if (inventory == playerWeaponInventory) {
            updateInventoryStatus(-1, inventory, 'W');
            removeAbilitiesFromLoadout();
            UI_Controller.updateInventoryIcon(UI_Controller.weaponInventoryButtons, targetIdx, null, 0);
        }
        else if (inventory == playerPowerupInventory) {
            updateInventoryStatus(-1, inventory, 'P');
            UI_Controller.updateInventoryIcon(UI_Controller.powerupInventoryButtons, targetIdx, null, 0);
        }
    }

    // Removes all abilities associated with a weapon from the loadout and updates UI to remove those ability icons.
    public void removeAbilitiesFromLoadout() {
        List<int> slotsUsed = playerLoadout.currSlotsUsed;
        playerLoadout.removeAbilities();
        foreach (int abilityIdx in slotsUsed) {
            UI_Controller.updateAbilityIcon(abilityIdx, null, 0);
        }
    }

    // Updates a specific slot of an inventory.
    public void updateInventoryStatus(int targetIdx, Inventory inventory, char itemType) {
        inventory.selectCurrItem(targetIdx);
        UI_Controller.updateInventoryStatusUI(targetIdx, false, itemType);
    }

    // Like updateInventoryStatus(), but if the weapon that is selected was already selected, deselect it. 
    public void updateWeaponInventoryStatusSecure(int targetIdx) {
        if (targetIdx != playerWeaponInventory.currIdx || !playerWeaponInventory.selectedSlot) {
            playerWeaponInventory.selectCurrItem(targetIdx);
        }
        else {
            playerWeaponInventory.selectCurrItem(-1);
        }
        updateAbilities(targetIdx, playerWeaponInventory.selectedSlot);
        UI_Controller.updateInventoryStatusUI(targetIdx, true, 'W');
    }

    // Like updateInventoryStatus(), but if the powerup that is selected was already selected, deselect it.
    public void updatePowerupInventoryStatusSecure(int targetIdx) {
        if (targetIdx != playerPowerupInventory.currIdx || !playerPowerupInventory.selectedSlot) {
            playerPowerupInventory.selectCurrItem(targetIdx);
        }
        else {
            playerPowerupInventory.selectCurrItem(-1);
        }
        UI_Controller.updateInventoryStatusUI(targetIdx, true, 'P');
    }

    // Updates loadout information and UI to respond to player interaction. 
    public void updateLoadoutStatus(int targetIdx)
    {
        if (playerLoadout.useLoadoutAbility(targetIdx, gameObject))
        {
            UI_Controller.updateLoadoutStatusUI(targetIdx, false);
        }
    }

    // Updates the current abilities and their icons depending on the Weapon being held.
    public void updateAbilities(int targetIdx, bool selectedSlot) {
        removeAbilitiesFromLoadout();
        if (selectedSlot && playerWeaponInventory.items[targetIdx] is not null) {
            addAbilitiesToLoadout((Weapon)playerWeaponInventory.items[targetIdx]);
        }
    }

    // Update the link between the powerup and the weapon.
    /* public void updateLinks() {
    for (int i = 0; i < playerPowerupInventory.powerups.Count; i++) {
        if (playerPowerupInventory.powerups[i] is not null && playerWeaponInventory.weapons[i] is not null) {
            playerPowerupInventory.powerups[i].buff(playerWeaponInventory.weapons[i]);
        }
    } */

    // saving and loading functions
    public void GetPlayerData(ref Player_Save_Data data) { // uses a reference as a parameter rather than a normal one because we don't want to copy the data, we want to be able to modify the original data
        Debug.Log("GetPlayerData!");
        data.position = transform.position; // storing the player's position by modifying the original data from the struct
        data.health = playerHealth; // storing the player's health by modifying the original data from the struct
    }

    public void LoadPlayerData(Player_Save_Data data) { // getting the saved data as a parameter
        Debug.Log("LoadPlayerData!");
        Debug.Log("LoadPlayerData! Saved Health: " + data.health);
        transform.position = data.position; // moving the player to the saved position
        playerHealth = data.health; // setting the player's health to the saved health
        // Debug.Log("Player Health after loading: " + playerHealth);
    }
}
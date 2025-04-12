using UnityEngine;
using InteractableManager;
using UIManager;
using System.Collections.Generic;
using WeaponManager;
using PowerupManager;
using AbilityManager;
using Unity.Burst.Intrinsics;
// using System.Threading;

public class Player_Controller : MonoBehaviour
{
    public float playerForce = 20f;
    public float maxHealth = 100f;
    public float playerHealth;
    public float pickUpRange = 5f;
    private bool allowPlayerInput = true;
    private Vector2 lastPos2D;
    private Rigidbody playerRb;
    private WeaponInventory playerWeaponInventory;
    private PowerupInventory playerPowerupInventory;
    private Loadout playerLoadout;
    private UI_Manager UI_Controller;
    [HideInInspector] public Vector2 lastMovementDirection;
    private bool tested = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerHealth = maxHealth;
        UI_Controller = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
        playerWeaponInventory = new WeaponInventory(UI_Controller.weaponInventoryButtons.Count, new List<int> { 7, 8, 9, 0 });
        playerWeaponInventory.resetInventory();
        playerPowerupInventory = new PowerupInventory(UI_Controller.powerupInventoryButtons.Count, new List<string> { "c", "v", "b", "n", "m" });
        playerPowerupInventory.resetInventory();
        playerLoadout = new Loadout(UI_Controller.loadoutButtons.Count, new List<int> { 1, 2, 3, 4 });
        playerLoadout.resetLoadout();
        allowPlayerInput = true;
        UI_Controller.setUpMetricBars(maxHealth);
        UI_Controller.SetPlayerPowerupInventory(playerPowerupInventory);
        Ability permaDashAbility = new Dash(5, 10);
        playerLoadout.addAbility(permaDashAbility, false);
        UI_Controller.updateAbilityIcon(0, permaDashAbility.icon, 255);
    }

    private void test() {
        Weapon testWeapon = new Sword(new List<Ability>(){new Dash(5, 10)});
        Weapon testWeapon2 = new Sword(new List<Ability>(){new Dash(5, 10)});
        testWeapon.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0));
        testWeapon2.dropItem(new Vector3(4, 1, -6), Quaternion.Euler(0, 0, 0));

        Powerup testPowerup = new Fire(0);
        testPowerup.dropItem(new Vector3(-4, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup1 = new Fire(0);
        testPowerup1.dropItem(new Vector3(-5, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup2 = new Fire(0);
        testPowerup2.dropItem(new Vector3(-6, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup3 = new Fire(0);
        testPowerup3.dropItem(new Vector3(-7, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup4 = new Poision(0);
        testPowerup4.dropItem(new Vector3(-8, 1, -6), Quaternion.Euler(0, 0, 0));
    }

    // Moves the character.
    void MovePlayer(float forceX, float forceY, float forceZ)
    {
        playerRb.AddForce(forceX * Time.deltaTime, forceY * Time.deltaTime, forceZ * Time.deltaTime, ForceMode.VelocityChange);
    }

    // Allows character movement by player input.
    void InitPlayerMovement()
    {
        if (Input.GetKey("w"))
        {
            MovePlayer(0f, 0f, playerForce);
        }
        if (Input.GetKey("s"))
        {
            MovePlayer(0f, 0f, -playerForce);
        }
        if (Input.GetKey("a"))
        {
            MovePlayer(-playerForce, 0f, 0f);
        }
        if (Input.GetKey("d"))
        {
            MovePlayer(playerForce, 0f, 0f);
        }
    }

    // Checks for whether the player drops an Weapon, and then instantiates that object.
    public void checkForWeaponDrop() {
        if (Input.GetKeyDown("e")) {
            Weapon currWeapon = playerWeaponInventory.weapons[playerWeaponInventory.currIdx];
            Debug.Log("Weapon dropped!" + " " + currWeapon + " " + playerWeaponInventory.currIdx + " " + playerWeaponInventory.selectedSlot);
            if (playerWeaponInventory.selectedSlot && currWeapon is not null) {
                removeWeaponFromInventory(playerWeaponInventory.currIdx);
                float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
                currWeapon.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
                Quaternion.Euler(0, playerRotationY, 0));
            }
        }
    }
    // Checks for whether the player drops an Powerup, and then instantiates that object.
    public void checkForPowerupDrop() {
        if (Input.GetKeyDown("r")) {
            Powerup currPowerup = playerPowerupInventory.powerups[playerPowerupInventory.currIdx];
            Debug.Log("Powerup dropped!" + " " + currPowerup + " " + playerPowerupInventory.currIdx + " " + playerPowerupInventory.selectedSlot);
            if (playerPowerupInventory.selectedSlot && currPowerup is not null) {
                removePowerupFromInventory(playerPowerupInventory.currIdx);
                float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
                currPowerup.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
                Quaternion.Euler(0, playerRotationY, 0));
            }
        }
    }

    private void PlayerDied() {
        UI_Manager.instance.GameOver();
        gameObject.SetActive(false);
    }

    // Adds an Weapon to the inventory (replaces an Weapon if no inventory space remains) and updates UI to include the new Weapon icon.
    public void addWeaponToInventory(Weapon Weapon) {
        Weapon prevWeapon = playerWeaponInventory.weapons[playerWeaponInventory.currIdx];
        if (prevWeapon is not null && !playerWeaponInventory.spaceInInventory()) {
            removeWeaponFromInventory(playerWeaponInventory.currIdx);
            float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            prevWeapon.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
            Quaternion.Euler(0, playerRotationY, 0));
        }
        playerWeaponInventory.addWeapon(Weapon);
        updateWeaponInventoryStatus(playerWeaponInventory.currIdx);
        UI_Controller.updateWeaponIcon(playerWeaponInventory.currIdx, Weapon.object2D, 255);
        updateAbilities(playerWeaponInventory.currIdx, playerWeaponInventory.selectedSlot);
    }
    // Adds an Powerup to the inventory (replaces an Powerup if no inventory space remains) and updates UI to include the new Powerup icon.
    public void addPowerupToInventory(Powerup Powerup) {
        Powerup prevPowerup = playerPowerupInventory.powerups[playerPowerupInventory.currIdx];
        if (prevPowerup is not null && !playerPowerupInventory.spaceInInventory()) {
            removePowerupFromInventory(playerPowerupInventory.currIdx);
            float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            prevPowerup.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
            Quaternion.Euler(0, playerRotationY, 0));
        }
        playerPowerupInventory.addPowerup(Powerup);
        updatePowerupInventoryStatus(playerPowerupInventory.currIdx);
        UI_Controller.updatePowerupIcon(playerPowerupInventory.currIdx, Powerup.object2D, 255);
        // add the stat buffs to the weapon
    }

    // Adds all abilities associated with an Weapon to the loadout and updates UI to include the new ability icons.
    public void addAbilitiesToLoadout(Weapon weapon) {
        foreach (Ability ability in weapon.abilityList) {
            int abilityIdx = playerLoadout.addAbility(ability, true);
            UI_Controller.updateAbilityIcon(abilityIdx, ability.icon, 255);
        }
    }

    // Removes the currently selected Weapon from the inventory and removes the icon on that slot.
    public void removeWeaponFromInventory(int targetIdx) {
        playerWeaponInventory.removeWeapon(targetIdx);
        updateWeaponInventoryStatus(-1);
        UI_Controller.updateWeaponIcon(targetIdx, null, 0); // update this
        updateAbilities(targetIdx, playerWeaponInventory.selectedSlot);
    }
    // Removes the currently selected Powerup from the inventory and removes the icon on that slot.
    public void removePowerupFromInventory(int targetIdx) {
        playerPowerupInventory.removePowerup(targetIdx);
        updatePowerupInventoryStatus(-1);
        UI_Controller.updatePowerupIcon(targetIdx, null, 0); // update this
    }

    // Removes all abilities associated with an Weapon from the loadout and updates UI to remove those ability icons.
    public void removeAbilitiesFromLoadout() {
        List<int> slotsUsed = playerLoadout.currSlotsUsed;
        playerLoadout.removeAbilities();
        foreach (int abilityIdx in slotsUsed) {
            UI_Controller.updateAbilityIcon(abilityIdx, null, 0);
        }
    }

    // Updates weapon inventory information and UI to respond to player interaction.
    public void updateWeaponInventoryStatus(int targetIdx) {
        playerWeaponInventory.selectCurrWeapon(targetIdx);
        UI_Controller.updateWeaponInventoryStatusUI(targetIdx, false);
        updateAbilities(targetIdx, playerWeaponInventory.selectedSlot);
    }
    // Same as above function, however with this function, selecting on an already selected icon will deselect it instead.
    public void updateWeaponInventoryStatusSecure(int targetIdx) {
        if (targetIdx != playerWeaponInventory.currIdx || !playerWeaponInventory.selectedSlot) {
            playerWeaponInventory.selectCurrWeapon(targetIdx);
        } else {
            playerWeaponInventory.selectCurrWeapon(-1);
        }
        UI_Controller.updateWeaponInventoryStatusUI(targetIdx, true);
        updateAbilities(targetIdx, playerWeaponInventory.selectedSlot);
    }

    // Updates powerup inventory information and UI to respond to player interaction.
    public void updatePowerupInventoryStatus(int targetIdx) {
        playerPowerupInventory.selectCurrPowerup(targetIdx);
        UI_Controller.updatePowerupInventoryStatusUI(targetIdx, false);
    }
    // Same as above function, however with this function, selecting on an already selected icon will deselect it instead.
    public void updatePowerupInventoryStatusSecure(int targetIdx) {
        if (targetIdx != playerPowerupInventory.currIdx || !playerPowerupInventory.selectedSlot) {
            playerPowerupInventory.selectCurrPowerup(targetIdx);
        } else {
            playerPowerupInventory.selectCurrPowerup(-1);
        }
        UI_Controller.updatePowerupInventoryStatusUI(targetIdx, true);
    }

    // Updates loadout information and UI to respond to player interaction. 
    public void updateLoadoutStatus(int targetIdx)
    {
        if (playerLoadout.useAbility(targetIdx, gameObject))
        {
            UI_Controller.updateLoadoutStatusUI(targetIdx, false);
        }
    }

    // Updates the current abilities and their icons depending on the Weapon being held.
    public void updateAbilities(int targetIdx, bool selectedSlot) {
        removeAbilitiesFromLoadout();
        if (selectedSlot && playerWeaponInventory.weapons[targetIdx] is not null) {
            addAbilitiesToLoadout(playerWeaponInventory.weapons[targetIdx]);
        }
    }

    // public void updateLinks() {
    //    for (int i = 0; i < playerPowerupInventory.powerups.Count; i++) {
    //         if (playerPowerupInventory.powerups[i] is not null && playerWeaponInventory.weapons[i] is not null) {
    //             playerPowerupInventory.powerups[i].buff(playerWeaponInventory.weapons[i]);
    //         }
    //     }
    // }

    // Update function used for all physics updates.
    void FixedUpdate()
    {
        if (allowPlayerInput)
        {
            InitPlayerMovement();
        }
    }

    // All other updates are in the standard Update() function, such as checking for other player inputs.
    void Update()
    {
        // All other updates are in the standard Update() function, such as checking for other player inputs.
        Vector2 playerPos2D = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        if (playerPos2D != lastPos2D) {
            lastMovementDirection = (playerPos2D-lastPos2D).normalized;
        }
        lastPos2D = playerPos2D;
        if (tested) {
            test();
            tested = false;
        } 
        if (allowPlayerInput) {
            int weaponInventoryInput = playerWeaponInventory.checkKeyInput();
            int powerupInventoryInput = playerPowerupInventory.checkKeyInput();
            int loadoutInput = playerLoadout.checkKeyInput();
            if (weaponInventoryInput != -1) {
                updateWeaponInventoryStatusSecure(weaponInventoryInput);
                // updateAbilities(playerWeaponInventory.currIdx, playerWeaponInventory.selectedSlot);
            }
            if (powerupInventoryInput != -1) {
                updatePowerupInventoryStatusSecure(powerupInventoryInput);
            }
            if (loadoutInput > -1) {
                updateLoadoutStatus(loadoutInput);
            }
            UI_Controller.updateMetricBars(playerHealth);
            checkForWeaponDrop();
            checkForPowerupDrop();
        }
        UI_Controller.updateMetricBars(playerHealth);
        if (playerHealth <= 0) {
            allowPlayerInput = false;
            Debug.Log("Player has died.");
            PlayerDied();
        }
        // playerHealth -= 0.25f;
        // Debug.Log("Player Health: " + playerHealth);
    }

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
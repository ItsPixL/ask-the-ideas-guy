using UnityEngine;
using InteractableManager;
using UIManager;
using System.Collections.Generic;
using ItemManager;
using AbilityManager;
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
    private Inventory playerInventory;
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
        playerInventory = new Inventory(UI_Controller.inventoryButtons.Count, new List<int> { 6, 7, 8, 9, 0 });
        playerLoadout = new Loadout(UI_Controller.loadoutButtons.Count, new List<int> { 1, 2, 3, 4 });
        playerInventory.resetInventory();
        playerLoadout.resetLoadout();
        allowPlayerInput = true;
        UI_Controller.setUpMetricBars(maxHealth);
        Ability permaDashAbility = new Dash(5, 10);
        playerLoadout.addAbility(permaDashAbility, false);
        UI_Controller.updateAbilityIcon(0, permaDashAbility.icon, 255);
    }

    private void test() {
        Item testItem = new Sword(new List<Ability>(){new Dash(5, 10)});
        Item testItem2 = new Sword(new List<Ability>(){new Dash(5, 10)});
        testItem.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0));
        testItem2.dropItem(new Vector3(4, 1, -6), Quaternion.Euler(0, 0, 0));
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

    // Checks for whether the player drops an item, and then instantiates that object.
    public void checkForDrop() {
        if (Input.GetKeyDown("e")) {
            Item currItem = playerInventory.items[playerInventory.currIdx];
            if (playerInventory.selectedSlot && currItem is not null) {
                removeItemFromInventory(playerInventory.currIdx);
                float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
                currItem.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
                Quaternion.Euler(0, playerRotationY, 0));
            }
        }
    }

    private void PlayerDied() {
        UI_Manager.instance.GameOver();
        gameObject.SetActive(false);
    }

    // Adds an item to the inventory (replaces an item if no inventory space remains) and updates UI to include the new item icon.
    public void addItemToInventory(Item item) {
        Item prevItem = playerInventory.items[playerInventory.currIdx];
        if (prevItem is not null && !playerInventory.spaceInInventory()) {
            removeItemFromInventory(playerInventory.currIdx);
            float playerRotationY = Vector3.SignedAngle(new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            prevItem.dropItem(gameObject.transform.position-new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y),
            Quaternion.Euler(0, playerRotationY, 0));
        }
        playerInventory.addItem(item);
        updateInventoryStatus(playerInventory.currIdx);
        UI_Controller.updateItemIcon(playerInventory.currIdx, item.object2D, 255);
        updateAbilities(playerInventory.currIdx, playerInventory.selectedSlot);
    }

    // Adds all abilities associated with an item to the loadout and updates UI to include the new ability icons.
    public void addAbilitiesToLoadout(Item item) {
        foreach (Ability ability in item.abilityList) {
            int abilityIdx = playerLoadout.addAbility(ability, true);
            UI_Controller.updateAbilityIcon(abilityIdx, ability.icon, 255);
        }
    }

    // Removes the currently selected item from the inventory and removes the icon on that slot.
    public void removeItemFromInventory(int targetIdx) {
        playerInventory.removeItem(targetIdx);
        updateInventoryStatus(-1);
        UI_Controller.updateItemIcon(targetIdx, null, 0);
        updateAbilities(targetIdx, playerInventory.selectedSlot);
    }

    // Removes all abilities associated with an item from the loadout and updates UI to remove those ability icons.
    public void removeAbilitiesFromLoadout() {
        List<int> slotsUsed = playerLoadout.currSlotsUsed;
        playerLoadout.removeAbilities();
        foreach (int abilityIdx in slotsUsed) {
            UI_Controller.updateAbilityIcon(abilityIdx, null, 0);
        }
    }

    // Updates inventory information and UI to respond to player interaction.
    public void updateInventoryStatus(int targetIdx) {
        playerInventory.selectCurrItem(targetIdx);
        UI_Controller.updateInventoryStatusUI(targetIdx, false);
        updateAbilities(targetIdx, playerInventory.selectedSlot);
    }

    // Same as above function, however with this function, selecting on an already selected icon will deselect it instead.
    public void updateInventoryStatusSecure(int targetIdx) {
        if (targetIdx != playerInventory.currIdx || !playerInventory.selectedSlot) {
            playerInventory.selectCurrItem(targetIdx);
        }
        else {
            playerInventory.selectCurrItem(-1);
        }
        UI_Controller.updateInventoryStatusUI(targetIdx, true);
        updateAbilities(targetIdx, playerInventory.selectedSlot);
    }

    // Updates loadout information and UI to respond to player interaction. 
    public void updateLoadoutStatus(int targetIdx)
    {
        if (playerLoadout.useAbility(targetIdx, gameObject))
        {
            UI_Controller.updateLoadoutStatusUI(targetIdx, false);
        }
    }

    // Updates the current abilities and their icons depending on the item being held.
    public void updateAbilities(int targetIdx, bool selectedSlot) {
        removeAbilitiesFromLoadout();
        if (selectedSlot && playerInventory.items[targetIdx] is not null) {
            addAbilitiesToLoadout(playerInventory.items[targetIdx]);
        }
    }

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
            int inventoryInput = playerInventory.checkKeyInput();
            int loadoutInput = playerLoadout.checkKeyInput();
            if (inventoryInput != -1) {
                updateInventoryStatusSecure(inventoryInput);
                // updateAbilities(playerInventory.currIdx, playerInventory.selectedSlot);
            }
            if (loadoutInput > -1) {
                updateLoadoutStatus(loadoutInput);
            }
            UI_Controller.updateMetricBars(playerHealth);
            checkForDrop();
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
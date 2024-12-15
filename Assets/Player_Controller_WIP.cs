using UnityEngine;
using ItemManager;
using System.Collections.Generic;

public class Inventory {
    public List<Item> items;
    private int maxSlots;
    private int currIdx = 0;
    private Item currItem;

    public Inventory(int maxSlots) {
        this.maxSlots = maxSlots;
    }

    public void resetInventory() { 
        items = new List<Item>(new Item[maxSlots]);
    }

    private int keepIdxInRange(int currIdx) {
        if (currIdx < 0) {
            return maxSlots+currIdx;
        }
        if (currIdx >= maxSlots) {
            return maxSlots-currIdx;
        }
        return currIdx;
    } 

    public void navigateInventory() {
        if (Input.GetKeyDown("e")) {
            currIdx += 1;
        }
        if (Input.GetKeyDown("q")) {
            currIdx -= 1;
        }
        for (int i = 1; i <= maxSlots; i++) {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i))) {
                currIdx = i-1;
                // Debug.Log(currIdx);
            }
        }
        currIdx = keepIdxInRange(currIdx);
    }

    public Item findCurrentItem() {
        if (items[currIdx] == null) {
            return null;
        }
        return items[currIdx];
    }   
}

public class Player_Controller_WIP : MonoBehaviour {
    public Rigidbody rb;
    public float force = 5f;
    private bool allowPlayerInput = true;
    private Inventory playerInventory = new Inventory(3);
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start() {
        // Nothing here yet!
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

    void FixedUpdate() {
        if (allowPlayerInput) {
            // All player input functions should be put here.
            InitPlayerMovement();
        }
    }

    void Update() {
        if (allowPlayerInput) {
            playerInventory.navigateInventory();
        }
    }
}

using UnityEngine;
using TMPro;
using InteractableManager;
// using NPCInteractable;

public class Pick_Mechanic : MonoBehaviour
{
    public static TMP_Text pickupText;
    public GameObject player;
    private bool playerNearby = false; 
    private static Player_Controller playerController; 
    private static Update_Closest_Item closestItemScript;
    public Item itemRef;
    private static LayerMask playerLayerMask;
    private float detectionRadius;
    // for npc interactions
    public LayerMask npcLayerMask;
    public float interactRange = 2f; // The range within which the player can interact with NPCs

    void Start() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<Player_Controller>();
        closestItemScript = player.GetComponent<Update_Closest_Item>();
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadius = playerController.pickUpRange;
        if (pickupText == null) {
            pickupText = GameObject.Find("Canvas/pickupText")?.GetComponent<TMP_Text>(); // Finds pickupText automatically
        }
        showTextUI(false);
    }

    // Update is called once per frame
    void Update() {
        playerNearby = canPickObject();
        if (playerNearby) { 
            showTextUI(true); 
            if (Input.GetKeyDown("f")) { 
                showTextUI(false);
                if (gameObject == closestItemScript.closestObject) {
                    if (itemRef is Weapon weapon) {
                        playerController.addItemToInventory(weapon, playerController.playerWeaponInventory);
                        itemRef.pickItem();
                        closestItemScript.objectsOfConcern.Remove(gameObject);
                        Destroy(gameObject);
                    }
                    else if (itemRef is Powerup powerup) {
                        playerController.addItemToInventory(powerup, playerController.playerPowerupInventory);
                        itemRef.pickItem();
                        closestItemScript.objectsOfConcern.Remove(gameObject);
                        Destroy(gameObject);
                    }
                }
            }
        }
        else {
            if (closestItemScript.objectsOfConcern.Contains(gameObject)) {
                closestItemScript.objectsOfConcern.Remove(gameObject);
            }
            showTextUI(false); 
        }
        if (Input.GetKeyDown("g")) { // Interact with NPCs
        Debug.Log("G key pressed for NPC interaction.");
            TryInteractWithNPC();
        }
    }

    void TryInteractWithNPC() {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, npcLayerMask);
        foreach (var hit in hits) {
            NPCInteractable npc = hit.GetComponent<NPCInteractable>();
            if (npc != null) {
                npc.Interact();
                break; // Only interact with the first NPC
            }
        }
    }

    // Checks if a player is nearby and if the object can be picked.
    public bool canPickObject() {
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, detectionRadius, playerLayerMask);

        foreach (Collider hit in hits) {
            if (hit.CompareTag("Player")) {
                if (!closestItemScript.objectsOfConcern.Contains(gameObject)) {
                    closestItemScript.objectsOfConcern.Add(gameObject);
                }
                return true;
            }
        }
        return false; 
    }

    public void showTextUI(bool state) {
        if (pickupText is not null) {
            pickupText.gameObject.SetActive(state);
        }
    }

    // Draws a sphere around the item to show the detection radius. Only works if you have selected the item from the hierarchy.
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, detectionRadius);
    }
}
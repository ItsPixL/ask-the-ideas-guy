using UnityEngine;
using TMPro;
using InteractableManager;
using NPCInteractableManager;

public class Pick_Mechanic : MonoBehaviour
{
    public TMP_Text pickupText;
    public GameObject player;
    private bool playerNearby = false; 
    private static Player_Controller playerController; 
    private static Update_Closest_Item closestItemScript;
    public Item itemRef;
    private static LayerMask playerLayerMask;
    private float detectionRadius;

    // NPC interaction settings
    public LayerMask npcLayerMask;
    public float interactRange = 2f;

    void Start() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<Player_Controller>();
        closestItemScript = player.GetComponent<Update_Closest_Item>();
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadius = playerController.pickUpRange;

        // if (pickupText == null) {
        //     pickupText = GameObject.Find("Canvas/pickupText")?.GetComponent<TMP_Text>();
        //     Debug.Log("Pickup text found: " + pickupText);
        // }
        showTextUI(false);
    }

    void Update() {
        playerNearby = canPickObject();

        if (Input.GetKeyDown("f")) {
            // trying NPC interaction
            bool interactedWithNPC = TryInteractWithNPC(); // having a flag to check if player interacted with NPC, and re-initialising it every time

            // Only continue to item interaction if player DIDN'T interact with NPC
            if (!interactedWithNPC && itemRef != null && gameObject == closestItemScript.closestObject) {
                if (itemRef is Weapon weapon) {
                    playerController.addItemToInventory(weapon, playerController.playerWeaponInventory);
                } else if (itemRef is Powerup powerup) {
                    playerController.addItemToInventory(powerup, playerController.playerPowerupInventory);
                }

                itemRef.pickItem();
                closestItemScript.objectsOfConcern.Remove(gameObject);
                Destroy(gameObject);
            }
        }

        // Remove item from concern if player is far away
        // if (!playerNearby && closestItemScript.objectsOfConcern.Contains(gameObject)) {
        //     closestItemScript.objectsOfConcern.Remove(gameObject);
        //     showTextUI(false);
        // } else if (playerNearby && gameObject == closestItemScript.closestObject) { // showing the text ui this way to make it so that the text isn't dependent on one item but rather the closest item
        //     showTextUI(true);
        // } else {
        //     showTextUI(false);
        // }
        if (!playerNearby && closestItemScript.objectsOfConcern.Contains(gameObject)) {
            // Player walked away from this item
            closestItemScript.objectsOfConcern.Remove(gameObject);
            showTextUI(false);
        }
        else if (playerNearby) {
            // Player is near this item
            if (gameObject == closestItemScript.closestObject) {
                // This is the closest item
                showTextUI(true);
                Debug.Log("I am the closest item: " + gameObject.name);
            } else {
                // Player is near but this isn't the closest item
                showTextUI(false);
            }
        }
    }

    bool TryInteractWithNPC() {
        Collider[] npchits = Physics.OverlapSphere(player.transform.position, interactRange, npcLayerMask); // creates the list everytime so the list isn't persistent
        if (npchits.Length == 0) {
            return false; // No NPCs nearby
        }
        foreach (var npchit in npchits) {
            NPCInteractable npc = npchit.GetComponent<NPCInteractable>();
            if (npc != null) {
                npc.Interact();
                return true; // Interaction happened
            }
        }
        return false; // No NPC nearby
    }

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
        } else {
            Debug.LogWarning("pickupText is not set on " + gameObject.name);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, detectionRadius);
    }
}

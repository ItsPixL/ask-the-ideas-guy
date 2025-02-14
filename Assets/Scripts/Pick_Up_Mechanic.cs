using UnityEngine;
using TMPro;
using InteractableManager;

public class Pick_Mechanic : MonoBehaviour
{
    public static TMP_Text pickupText; // it is static so that it can be used more than once
    private GameObject item_nearby; // Store the nearby item
    public Player_Controller playerController; // an instant of the player controller
    public LayerMask playerLayer = 1 << 8; // Set in Unity Inspector. This is used for the IsPlayerNearby() function, and is a way to filter objects by their layer, rather than their tag.
    public float detectionRadius; // The radius around the items that detects the player

    void Start() {
        if (pickupText == null) { // making sure it is only found once
            playerController = FindObjectOfType<Player_Controller>(); // Finds Player_Controller automatically
            pickupText = GameObject.Find("Canvas/pickupText")?.GetComponent<TMP_Text>(); // Finds pickupText automatically

            if (pickupText == null) {
                Debug.LogError("pickupText not found in the scene!");
            }
        }
        pickupText.gameObject.SetActive(false); // Hide the message initially
    }

    // Checks if a player is nearby.
    public GameObject IsPlayerNearby() {
        // Detect colliders within the detection radius on the playerLayer
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        // Debugging: Log the detected objects (if any)
        foreach (Collider hit in hits) {
            if (hit.CompareTag("Player")) { // Ensure the detected object is the player, just in case
                return hit.gameObject; // Return the detected player
            }
        }
        return null; // Return null if no player is detected
    }

    // Draws a sphere around the item to show the detection radius. Only works if you have selected the item from the hierarchy.
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Update is called once per frame
    void Update() {
        item_nearby = IsPlayerNearby();

        if (item_nearby != null) { // If player is in range
            pickupText.gameObject.SetActive(true); // Show UI

            if (Input.GetKeyDown(KeyCode.E)) { // If player presses 'E'
                Item item = new Item(gameObject.name, gameObject.tag);
                Destroy(gameObject); // Remove item
                pickupText.gameObject.SetActive(false); // Hide UI
            }
        }
        else {
            pickupText.gameObject.SetActive(false); // Hide UI when player is not nearby
        }
    }
}
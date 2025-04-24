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
    // public LayerMask npcLayerMask;
    // public float interactRange = 2f;

    void Start() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<Player_Controller>();
        closestItemScript = player.GetComponent<Update_Closest_Item>();
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadius = playerController.pickUpRange;
    }

    void Update() {
        playerNearby = canPickObject(closestItemScript.closestObject); // check if player is nearby

        if (Input.GetKeyDown("f")) {

            // Only continue to item interaction if player DIDN'T interact with NPC
            if (itemRef != null && gameObject == closestItemScript.closestObject) {
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

        // Clean up objects of concern if out of range
        if (!playerNearby && closestItemScript.objectsOfConcern.Contains(gameObject)) {
            closestItemScript.objectsOfConcern.Remove(gameObject);
        }
    }

    public bool canPickObject(GameObject currentClosestObject) {
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
        // else {
        //     Debug.LogWarning("pickupText is not set on " + gameObject.name);
        // }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, detectionRadius);
    }
}

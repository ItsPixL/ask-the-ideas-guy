using UnityEngine;
using TMPro;
using InteractableManager;

public class Pick_Mechanic : MonoBehaviour {
    public GameObject player;
    private bool playerNearby = false; 
    private static Player_Controller playerController; 
    private static Update_Closest_Item closestItemScript;
    public Item itemRef;
    private static LayerMask playerLayerMask;
    private float detectionRadius;

    void Start() {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Player_Controller>();
        closestItemScript = player.GetComponent<Update_Closest_Item>();
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadius = playerController.pickUpRange;
    }

    void Update() {
        playerNearby = canPickObject(); // check if player is nearby

        if (Input.GetKeyDown("f")) { // check if player presses the pick up key
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

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, detectionRadius);
    }
}

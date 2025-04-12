using UnityEngine;
using TMPro;
using InteractableManager;

public class Pick_Mechanic : MonoBehaviour
{
    public static TMP_Text pickupText;
    private bool playerNearby = false; 
    private static Player_Controller playerController; 
    private static Update_Closest_Item closestItemScript;
    public Item itemRef; 
    private static LayerMask playerLayerMask; 
    private float detectionRadius;

    void Start() {
        playerController = GameObject.Find("Player").GetComponent<Player_Controller>();
        closestItemScript = GameObject.Find("Player").GetComponent<Update_Closest_Item>();
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadius = playerController.pickUpRange;
        if (pickupText == null) { // 
            pickupText = GameObject.Find("Canvas/pickupText")?.GetComponent<TMP_Text>(); // Finds pickupText automatically
        }
        showTextUI(false);
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

    // Update is called once per frame
    void Update() {
        playerNearby = canPickObject();
        if (playerNearby) { 
            showTextUI(true); 
            if (Input.GetKeyDown("f")) { 
                showTextUI(false);
                if (gameObject == closestItemScript.closestObject) {
                    if (itemRef is Weapon weapon) {
                        playerController.addWeaponToInventory(weapon);
                    }
                    else if (itemRef is Powerup powerup) {
                        playerController.addPowerupToInventory(powerup);
                    }
                    itemRef.pickItem();
                    closestItemScript.objectsOfConcern.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
        }
        else {
            if (closestItemScript.objectsOfConcern.Contains(gameObject)) {
                closestItemScript.objectsOfConcern.Remove(gameObject);
            }
            showTextUI(false); 
        }
    }
}
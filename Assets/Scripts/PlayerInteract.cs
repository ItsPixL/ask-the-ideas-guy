using UnityEngine;
using NPCInteractableManager;
using InteractableManager;


public class PlayerInteract : MonoBehaviour {
    private float interactRange = 2f; // Define the interaction range
    private float pickUpRange = 3f;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in hitColliders) {
                if (collider.TryGetComponent(out NPCInteractable npc)) {
                    npc.Interact();// Interact with the NPC
                }
            }
        }
    }
    public NPCInteractable GetInteractableNPCObject() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in hitColliders) {
            if (collider.TryGetComponent(out NPCInteractable npc)) {
                return npc; // Return the first interactable NPC found
            }
        }
        return null; // No NPC nearby
    }
    public bool GetInteractablePOWERUPObject() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider collider in hitColliders) {
            if (collider.CompareTag("Powerup")) {
                return true; // Return the first interactable Powerup found
            }
        }
        return false; // No Powerup nearby
    }
    public bool GetInteractableWEAPONObject() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider collider in hitColliders) {
            if (collider.CompareTag("Weapon")) {
                return true; // Return the first interactable Weapon found
            }
        }
        return false; // No Weapon nearby
    }
}

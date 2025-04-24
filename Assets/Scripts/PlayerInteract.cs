using UnityEngine;
using NPCInteractableManager; // Import the namespace for NPCInteractable

public class PlayerInteract : MonoBehaviour {

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            float interactRange = 2f; // Define the interaction range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in hitColliders) {
                if (collider.TryGetComponent(out NPCInteractable npc)) {
                    npc.Interact();// Interact with the NPC
                }
            }
        }
    }
    public NPCInteractable GetInteractableObject() {
        float interactRange = 2f; // Define the interaction range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in hitColliders) {
            if (collider.TryGetComponent(out NPCInteractable npc)) {
                return npc; // Return the first interactable NPC found
            }
        }
        return null; // No NPC nearby
    }
}

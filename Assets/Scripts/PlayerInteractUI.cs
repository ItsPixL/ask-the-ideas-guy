using UnityEngine;

public class PlayerInteractUI : MonoBehaviour {
    [SerializeField] private GameObject NPCcontainerGameObject; // The container that has the NPC UI elements
    [SerializeField] private GameObject ITEMcontainerGameObject; // The container that has the ITEM UI elements
    [SerializeField] private PlayerInteract playerInteract; // Reference to the pick-up mechanic script
    private void Update() {
        if (playerInteract.GetInteractableNPCObject() != null) {
            ShowNPC(); // Show the UI if the player is near an interactable object
            HideITEM(); // Hide the UI if the player is not near an interactable object
        } else if (playerInteract.GetInteractablePOWERUPObject() != false) {
            ShowITEM(); // Show the UI if the player is near an interactable object
            HideNPC(); // Hide the UI if the player is not near an interactable object
        } else if (playerInteract.GetInteractableWEAPONObject() != false) {
            ShowITEM(); // Show the UI if the player is near an interactable object
            HideNPC(); // Hide the UI if the player is not near an interactable object
        } else {
            HideITEM(); // Hide the UI if the player is not near an interactable object
            HideNPC(); // Hide the UI if the player is not near an interactable object
        }
    }
    void ShowNPC() {
        NPCcontainerGameObject.SetActive(true); // Show the NPC UI container
    }

    void HideNPC() {
        NPCcontainerGameObject.SetActive(false); // Hide the NPC UI container
    }
    void ShowITEM() {
        ITEMcontainerGameObject.SetActive(true); // Show the ITEM UI container
    }

    void HideITEM() {
        ITEMcontainerGameObject.SetActive(false); // Hide the ITEM UI container
    }
}

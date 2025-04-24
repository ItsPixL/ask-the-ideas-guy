using UnityEngine;

public class PlayerInteractUI : MonoBehaviour {
    [SerializeField] private GameObject containerGameObject; // The container that has all of the UI elements
    [SerializeField] private PlayerInteract playerInteract; // Reference to the pick-up mechanic script
    private void Update() {
        if (playerInteract.GetInteractableObject() != null) {
            Show(); // Show the UI if the player is near an interactable object
        } else {
            Hide(); // Hide the UI if the player is not near an interactable object
        }
    }
    void Show() {
        containerGameObject.SetActive(true); // Show the UI container
    }

    void Hide() {
        containerGameObject.SetActive(false); // Hide the UI container
    }
}

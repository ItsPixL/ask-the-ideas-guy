using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DoorInterable : MonoBehaviour, IInteractable {
    private Animator animator; // Reference to the Animator component
    private bool isOpen = false; // Flag to track if the door is open or closed

    void Awake() {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the door
        if (animator == null) {
            Debug.LogError("Animator component not found on the door. Please assign an Animator component to the door.");
        }
    }

    public void ToggleDoor() {
        if (animator != null) {
            isOpen = !isOpen; // Toggle the door state
            animator.SetBool("IsOpen", isOpen); // Set the "IsOpen" parameter in the Animator to control the door animation
        }
    }

    public void Interact() {
        ToggleDoor(); // Call the method to toggle the door state when interacted with
    }
}

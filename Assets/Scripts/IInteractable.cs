using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable { // implementing an interface for all interactable objects in the game
// currently only applicable for NPCs and doors
    void Interact();
}

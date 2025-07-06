using UnityEngine;

namespace NPCInteractableManager {
    // Defines the basics of every NPC in the game.
    public class NPCInteractable : MonoBehaviour, IInteractable {
        public string npcName; // The name of the NPC
        public string npcDialogue; // The dialogue of the NPC

        private void Start() {
            // Initialization code can go here if needed
        }

        private void Update() {
            // Update code can go here if needed
        }

        public void Interact() {
            // Code to handle interaction with the NPC, such as displaying dialogue
            Debug.Log($"Interacting with {npcName}: {npcDialogue}");
        }
    }
}
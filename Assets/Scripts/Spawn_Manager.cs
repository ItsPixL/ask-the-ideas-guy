using UnityEngine;

public class Spawn_Manager : MonoBehaviour {
    public static Spawn_Manager instance { get; private set; } // creating an instance for ease of use across files
    void Awake() { // creating a singleton pattern to ensure only one instance of the spawn manager is running
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // this will prevent the spawn manager from being destroyed when a new scene is loaded
        } else if (instance != this) {
            Debug.LogWarning("Duplicate Spawn_Manager detected and destroyed."); // logged as a warning so it stands out
            Destroy(gameObject);
        }
    }
}
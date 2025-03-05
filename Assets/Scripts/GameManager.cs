// used for managing the state and scene of the game
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; } // creating an instance for ease of use across files. the get; private set; allows only the actual game manager to edit the instance, but the others can simply see it
    public static event System.Action<GameState> OnGameStateChanged; // creating an event to handle the game state. this allows other scripts to react when a game state changes. whenever the game state changes, we trigger this event
    private GameState currentGameState;
    public Player_Controller Player { get; private set; } // creating a property for the player for save data

    void Awake() { // creating a singleton pattern to ensure only one instance of the game manager is running
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // this will prevent the game manager from being destroyed when a new scene is loaded
        } else if (instance != this) {
            Debug.LogWarning("Duplicate Game_Manager detected and destroyed."); // logged as a warning so it stands out
            Destroy(gameObject);
        }
        // Initialize with a default state
        currentGameState = GameState.MainMenu;
    }

    public void UpdateGameState(GameState gameState) { // updating the game state
    currentGameState = gameState; // updating to the current game state
        switch (gameState) {
            case GameState.MainMenu:
                Time.timeScale = 1; // setting the time scale to 1 so the game runs at normal speed
                break;
            case GameState.InGame:
                Time.timeScale = 1; // setting the time scale to 1 so the game runs at normal speed
                break;
            case GameState.Paused:
                Time.timeScale = 0; // setting the time scale to 0 so the game is paused
                break;
            case GameState.GameOver:
                Time.timeScale = 0; // setting the time scale to 0 so the game is paused
                break;
        }
        OnGameStateChanged?.Invoke(gameState); // invoking the event, basically preventing a null error from occurring
    }

    public GameState CheckGameState() {
        return currentGameState;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) { // if the player presses the o key
            Debug.Log("Save Game"); // log that the game is being saved
            SaveSystem.Save(); // Save the game
        }
        if (Input.GetKeyDown(KeyCode.P)) { // if the player presses the p key
            Debug.Log("Load Game"); // log that the game is being loaded
            SaveSystem.Load(); // load the game
        }
    }
}

// whenever a new game state (like completing a level or discovering an item or chatting with an npc) is added, it should be added here
public enum GameState { // creating an enum to manage the state of the game. an enum is a list of named integer constants
    MainMenu, // 0
    InGame, // 1
    Paused, // 2
    GameOver // 3
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    void Awake() { // subscribing to the event when the game starts
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    void OnDestroy() { // unsubscribing from the event when the game ends
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    private void GameManager_OnGameStateChanged(GameState gameState) { // updating the game state
        gameObject.SetActive(gameState == GameState.MainMenu);
    }
    public void PlayGame() {
        Debug.Log("Play Game");
        if (GameManager.instance != null) {
            GameManager.instance.UpdateGameState(GameState.InGame); // Change the game state to 'InGame' (triggers the event)
            Debug.Log("GameManager instance has been initialized.");
        } else {
            Debug.LogError("GameManager instance is not initialized.");
        }
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        Debug.Log("END Game");
        Application.Quit();
    }
}

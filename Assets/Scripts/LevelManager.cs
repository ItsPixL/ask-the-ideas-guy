using UnityEngine;
using UnityEngine.SceneManagement;
using UIManager;

public class LevelManager : MonoBehaviour {
    public void PlayGame() { // going from the main menu to the game
        Debug.Log("Play Game");
        SceneManager.LoadScene("SampleScene");
        GameManager.instance.UpdateGameState(GameState.InGame); // Change the game state to 'InGame' (triggers the event)
    }
    public void NextScene() { // going to the next level.
        Debug.Log("Next Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PreviousScene() { // going to the previous level.
        Debug.Log("Previous Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void QuitGame() { // closing the game
        Debug.Log("END Game");
        Application.Quit();
    }
    public void ResumeCurrentScene() { // restarting the level. make it resume the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame() { // restarting the game to main menu. but later on you also have to delete the save data.
        SceneManager.LoadScene("Menu");
        GameManager.instance.UpdateGameState(GameState.MainMenu);
    }

    public void ToMainMenu() { // going back to the main menu without losing all of the save data.
        SceneManager.LoadScene("Menu");
        GameManager.instance.UpdateGameState(GameState.MainMenu);
    }
    public void ResumeGame() { // resuming the game
        UI_Manager.instance.Resume();
    }
}

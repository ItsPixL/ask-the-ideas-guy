using UnityEngine;
using UIManager;
using UnityEngine.SceneManagement;

public class LevelManagerScript : MonoBehaviour {
    public static LevelManagerScript instance { get; private set; }

    private void Awake() { // this insures that only one instance of levelmanagerscript is currently running
        if (LevelManagerScript.instance == null) {
            LevelManagerScript.instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Debug.LogWarning("Duplicate Level_Manager detected and destroyed.");
            Destroy(gameObject);
        }
    }

    // public void GameOver() { // showing or disabling the ui when the player dies
    //     UI_Manager _ui = GetComponent<UI_Manager>();
    //     if (_ui != null) {
    //         _ui.ToggleDeathPanel();
    //     }
    // }

    // both of these are only being used here. you can move them into game manager and use them from there.
    public void RestartGame() { // restarting the game
        SceneManager.LoadScene(0);
    }

    public void ResumeCurrentScene() { // restarting the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

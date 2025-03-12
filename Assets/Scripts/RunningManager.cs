using UnityEngine;
using UnityEngine.SceneManagement;

public class RunningManager : MonoBehaviour {
    // force running the managers scene
    static bool isInitialised = false; // ensures that the running manager is only initialised once
    void Awake() {
        if (isInitialised) {
            return;
        }
        isInitialised = true;
        string managersScene = "ManagersScene";
        string mainMenuScene = "Menu";
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != managersScene) {
            Debug.Log("Loading ManagersScene...");
            SceneManager.LoadScene(managersScene, LoadSceneMode.Additive); // runs the managers scene in the background
            Debug.Log("Finished Loading ManagersScene");
        }

        if (currentScene.name != mainMenuScene) {
            Debug.Log("Loading MainMenu...");
            SceneManager.LoadScene(mainMenuScene);
            Debug.Log("Finished Loading MainMenu");
        }
    }
}

using UnityEngine;
using UIManager;

public class LevelManagerScript : MonoBehaviour {
    public static LevelManagerScript instance;

    private void Awake() { // this insures that only one instance of levelmanagerscript is currently running
        if (LevelManagerScript.instance == null) {
            LevelManagerScript.instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void GameOver() { // showing or disabling the ui when the player dies
        UI_Manager _ui = GetComponent<UI_Manager>();
        if (_ui != null) {
            _ui.ToggleDeathPanel();
        }
    }
}

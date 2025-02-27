// using UnityEngine;

// public class PauseMenu : MonoBehaviour {
//     public static bool GameIsPaused = false;
//     public GameObject pauseMenuUI;

//     public void Resume() {
//         Debug.Log("Resuming game");
//         pauseMenuUI.SetActive(false);
//         Time.timeScale = 1f;
//         GameIsPaused = false;
//     }

//     public void Pause() {
//         Debug.Log("Pausing game");
//         pauseMenuUI.SetActive(true);
//         Time.timeScale = 0f;
//         GameIsPaused = true;
//     }

//     // Update is called once per frame
//     void Update() {
//         if (Input.GetKeyDown(KeyCode.Escape)) {
//             if (GameIsPaused) {
//                 Resume();
//             } else {
//                 Pause();
//             }
//         }
//     }
// }

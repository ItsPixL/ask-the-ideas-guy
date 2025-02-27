using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UIManager {
    // Handles the UI of the inventory.
    public class UI_Inventory {
        private List<Button> buttons;
        private Color normalOutlineColour;
        private Color selectedOutlineColour;
        public int currSelected = -1;

        public UI_Inventory(List<Button> buttons, Color normalOutlineColour, Color selectedOutlineColour) {
            this.buttons = buttons;
            this.normalOutlineColour = normalOutlineColour;
            this.selectedOutlineColour = selectedOutlineColour;
        }

        // Highlights the outline of the selected item slot (if any) in yellow, and leave the rest of the outlines black.
        public void selectCurrItem(int newIdx) {
            if (currSelected != -1) {
                Button prevButton = buttons[currSelected];
                Outline prevOutline = prevButton.GetComponent<Outline>();
                prevOutline.effectColor = normalOutlineColour;
            }
            if (newIdx == currSelected) {
                currSelected = -1;
            }
            else {
                currSelected = newIdx;
            }
            if (currSelected != -1) {
                Button currButton = buttons[currSelected];
                Outline currOutline = currButton.GetComponent<Outline>();
                currOutline.effectColor = selectedOutlineColour;
            }
        }
    }

    // Handles the UI of the loadout (stores usable abilities).
    public class UI_Loadout {
        private List<Button> buttons;
        private Color enabledOutlineColour;
        private Color disabledOutlineColour;
        
        public UI_Loadout(List<Button> buttons, Color enabledOutlineColour, Color disabledOutlineColour) {
            this.buttons = buttons;
            this.enabledOutlineColour = enabledOutlineColour;
            this.disabledOutlineColour = disabledOutlineColour;
        }

        // Disables the button, which dims the background colour and changes the outline colour to red.
        public void useCurrAbility(int abilityIdx) {
            Button currButton = buttons[abilityIdx];
            Outline currOutline = currButton.GetComponent<Outline>();
            currButton.interactable = false;
            currOutline.effectColor = disabledOutlineColour;
        }

        // Re-enables the button, which reverses the UI changes made upon disabling it and allows it to be used again.
        public void enableAbility(int abilityIdx) {
            Button currButton = buttons[abilityIdx];
            Outline currOutline = currButton.GetComponent<Outline>();
            currButton.interactable = true;
            currOutline.effectColor = enabledOutlineColour;
        }
    }

    // Handles the UI of the metric bars, such as health bars etc. 
    public class MetricBar {
        private Slider barSlider;
        private Image barFill;
        private float minValue;
        private float maxValue;
        private Gradient colorGradient;

        public MetricBar(Slider barSlider, Image barFill, float minValue, float maxValue, Gradient colorGradient) {
            this.barSlider = barSlider;
            this.barFill = barFill;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.colorGradient = colorGradient;
        }

        // Initialises the bar to the right values.
        public void setUpBar() {
            barSlider.minValue = minValue;
            barSlider.maxValue = maxValue;
            barSlider.value = maxValue;
            barFill.color = colorGradient.Evaluate(1f);
        }

        // Updates the bar's values given the current metric value.
        public void updateBar(float currValue) {
            barSlider.value = currValue;
            if (barSlider.value < minValue) {
                barSlider.value = minValue;
            }
            else if (barSlider.value > maxValue) {
                barSlider.value = maxValue;
            }
            barFill.color = colorGradient.Evaluate(barSlider.normalizedValue);
        }
    }

    public class StateManager : MonoBehaviour {
        public void ReloadCurrentScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public class MainMenu : MonoBehaviour {
        public void PlayGame() {
            Debug.Log("Play Game");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void QuitGame() {
            Debug.Log("END Game");
            Application.Quit();
        }
    }

    // public class PauseMenu : MonoBehaviour {
    //     public static bool GameIsPaused = false;
    //     [SerializeField] private GameObject pauseMenuUI;

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

    public class UI_Manager : MonoBehaviour {
        public List<Button> inventoryButtons;
        public List<Button> loadoutButtons;
        private UI_Inventory playerInventoryUI;
        private UI_Loadout playerLoadoutUI;
        public Gradient healthBarGradient;
        public Gradient energyBarGradient;
        private MetricBar healthBarUI;
        private MetricBar energyBarUI;
        [SerializeField] GameObject deathPanel; // serialized field to allow the object to be set in the Unity Editor, but not accessible from other scripts. This is just for security
        public static bool GameIsPaused = false;
        [SerializeField] private GameObject pauseMenuUI;

        // Another Start() function that sets up the metric bars. Function is separate to the Start() since it requires arguments given from the player object.
        public void setUpMetricBars(float playerHealth, float playerEnergy) {
            Slider healthSlider = GameObject.Find("Health Bar").GetComponent<Slider>();
            Image healthSliderFill = healthSlider.transform.Find("Fill").gameObject.GetComponent<Image>();
            Slider energySlider = GameObject.Find("Energy Bar").GetComponent<Slider>();
            Image energySliderFill = energySlider.transform.Find("Fill").gameObject.GetComponent<Image>();
            healthBarUI = new MetricBar(healthSlider, healthSliderFill, 0, playerHealth, healthBarGradient);
            energyBarUI = new MetricBar(energySlider, energySliderFill, 0, playerEnergy, energyBarGradient);
            healthBarUI.setUpBar();
            energyBarUI.setUpBar();
        }

        void Start() {
            playerInventoryUI = new UI_Inventory(inventoryButtons, Color.black, Color.yellow);
            playerLoadoutUI = new UI_Loadout(loadoutButtons, new Color(0, 0, 0, 150), new Color(255, 0, 0, 255));
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (GameIsPaused) {
                    Resume();
                }
                else {
                    Pause();
                }
            }
        }

        // A connector function - this function is called from Player_Controller.cs and calls a function within UI_Inventory.
        public void updateInventoryStatusUI(int targetIdx) {
            playerInventoryUI.selectCurrItem(targetIdx);
        }

        // A connector function - this function is called from Player_Controller.cs and calls a function within UI_Loadout.
        public void updateLoadoutStatusUI(int targetIdx) {
            playerLoadoutUI.useCurrAbility(targetIdx);
        }

        // A connector function - this function is called from Player_Controller.cs and calls a function within MetricBar.
        public void updateMetricBars(float currHealth, float currEnergy) {
            healthBarUI.updateBar(currHealth);
            energyBarUI.updateBar(currEnergy);
        }
        public void LoadMenu() {
            Debug.Log("Loading menu...");
        }

        public void QuitGame() {
            Debug.Log("Quitting game...");
            Application.Quit();
        }

        public void ToggleDeathPanel() {
            deathPanel.SetActive(!deathPanel.activeSelf);
        }

        // pause menu stuff
        public void Resume() {
            Debug.Log("Resuming game");
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        public void Pause() {
            Debug.Log("Pausing game");
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        // death stuff
        public void GameOver() {
            UI_Manager _ui = GetComponent<UI_Manager>();
            if (_ui != null)
            {
                _ui.ToggleDeathPanel();
            }
        }
    }
}
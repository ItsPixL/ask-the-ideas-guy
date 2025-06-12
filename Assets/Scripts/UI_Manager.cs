using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using InteractableManager;

namespace UIManager {
    // Handles the UI of an inventory.
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
        public void selectCurrItem(int newIdx, bool toggle) {
            if (currSelected != -1) {
                Button prevButton = buttons[currSelected];
                if (prevButton != null) {
                    Outline prevOutline = prevButton.GetComponent<Outline>();
                    if (prevOutline != null) {
                        prevOutline.effectColor = normalOutlineColour;
                    } else {
                        Debug.LogWarning($"Button at index {currSelected} has no Outline component.");
                    }
                } else {
                    Debug.LogWarning($"Button at index {currSelected} is null.");
                }
            }
            // if (currSelected != -1) {
            //     Button prevButton = buttons[currSelected];
            //     Outline prevOutline = prevButton.GetComponent<Outline>();
            //     prevOutline.effectColor = normalOutlineColour;
            // }
            if (newIdx == currSelected && toggle) {
                currSelected = -1;
            }
            else {
                currSelected = newIdx;
            }
            if (currSelected != -1) {
                Button currButton = buttons[currSelected];
                if (currButton != null) {
                    Outline currOutline = currButton.GetComponent<Outline>();
                    if (currOutline != null) {
                        currOutline.effectColor = selectedOutlineColour;
                    } else {
                        Debug.LogWarning($"Button at index {currSelected} has no Outline component.");
                    }
                } else {
                    Debug.LogWarning($"Button at index {currSelected} is null.");
                }
            }
            // if (currSelected != -1) {
            //     Button currButton = buttons[currSelected];
            //     Outline currOutline = currButton.GetComponent<Outline>();
            //     currOutline.effectColor = selectedOutlineColour;
            // }
        }
    }

    // Handles the UI of the loadout (stores usable abilities).
    public class UI_Loadout{
        private List<Button> buttons;
        private Color enabledOutlineColour;
        private Color disabledOutlineColour;
        
        public UI_Loadout(List<Button> buttons, Color enabledOutlineColour, Color disabledOutlineColour) {
            this.buttons = buttons;
            this.enabledOutlineColour = enabledOutlineColour;
            this.disabledOutlineColour = disabledOutlineColour;
        }
        
        // Disables the button, which dims the background colour and changes the outline colour to red.
        public void useCurrAbility(int abilityIdx)
        {
            Button currButton = buttons[abilityIdx];
            Outline currOutline = currButton.GetComponent<Outline>();
            currButton.interactable = false;
            currOutline.effectColor = disabledOutlineColour;
            Image buttonImage = currButton.GetComponent<Image>(); // Shade the button to indicate it's disabled
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray shade
            }
        }

        // Re-enables the button, which reverses the UI changes made upon disabling it and allows it to be used again.
        public void enableAbility(int abilityIdx)
        {
            Button currButton = buttons[abilityIdx];
            Debug.Log("Enabling ability: " + currButton.name);
            Outline currOutline = currButton.GetComponent<Outline>();
            currButton.interactable = true;
            currOutline.effectColor = enabledOutlineColour;
            Image buttonImage = currButton.GetComponent<Image>(); // Restore the button's background color to its normal color
            if (buttonImage != null) {
                buttonImage.color = Color.white;
                Debug.Log("HI");
            }
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

    public class UI_Manager : MonoBehaviour {
        public List<Button> weaponInventoryButtons;
        public List<Button> powerupInventoryButtons;
        public List<Button> loadoutButtons;
        private UI_Inventory playerWeaponInventoryUI;
        private UI_Inventory playerPowerupInventoryUI;
        private PowerupInventory playerPowerupInventory;
        public UI_Loadout playerLoadoutUI;
        public Gradient healthBarGradient;
        private MetricBar healthBarUI;
        // private GameObject deathPanel;
        // private GameObject pauseMenu;
        public bool GameIsPaused = false;
        public static UI_Manager instance { get; private set; } // Singleton instance

        // Another Start() function that sets up the metric bars. Function is separate to the Start() since it requires arguments given from the player object.
        public void setUpMetricBars(float playerHealth) {
            Slider healthSlider = GameObject.Find("Health Bar").GetComponent<Slider>();
            Image healthSliderFill = healthSlider.transform.Find("Fill").gameObject.GetComponent<Image>();
            healthBarUI = new MetricBar(healthSlider, healthSliderFill, 0, playerHealth, healthBarGradient);
            healthBarUI.setUpBar();
        }

        void Start() {
            playerWeaponInventoryUI = new UI_Inventory(weaponInventoryButtons, Color.black, Color.yellow);
            playerPowerupInventoryUI = new UI_Inventory(powerupInventoryButtons, Color.black, Color.yellow);
            playerLoadoutUI = new UI_Loadout(loadoutButtons, new Color(0, 0, 0, 150), new Color(255, 0, 0, 255));
            updateInventoryIcon(powerupInventoryButtons, 0, null, 0);
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

        public void updateInventoryStatusUI(int targetIdx, bool toggle, char itemType) {
            if (itemType == 'W') {
                playerWeaponInventoryUI.selectCurrItem(targetIdx, toggle);
            }
            else if (itemType == 'P') {
                playerPowerupInventoryUI.selectCurrItem(targetIdx, toggle);
            }
        }

        public void updateInventoryIcon(List<Button> buttons, int targetIdx, Sprite newImage, int colorAlpha) {
            string iconName = "";
            if (buttons == weaponInventoryButtons) {
                iconName = "Weapon Icon";
            }
            else if (buttons == powerupInventoryButtons) {
                iconName = "Powerup Icon";
            }
            Button currButton = buttons[targetIdx];
            currButton.transform.Find(iconName).gameObject.GetComponent<Image>().sprite = newImage;
            currButton.transform.Find(iconName).gameObject.GetComponent<Image>().color = Color.black;
            Color iconColor = currButton.transform.Find(iconName).gameObject.GetComponent<Image>().color;
            iconColor.a = colorAlpha;
            currButton.transform.Find(iconName).gameObject.GetComponent<Image>().color = iconColor;
        }

        public void deletePowerupIcon(int targetIdx) {
            Button currButton = powerupInventoryButtons[targetIdx]; // Get the target button
            Transform powerupIconTransform = currButton.transform.Find("Powerup Icon"); // Find the Powerup Icon child

            if (powerupIconTransform != null) {
                Image powerupIconImage = powerupIconTransform.gameObject.GetComponent<Image>();
                if (powerupIconImage != null) {
                    powerupIconImage.sprite = null; // Remove the sprite
                    Color iconColor = powerupIconImage.color;
                    iconColor.a = 0; // Set alpha to 0 to make it invisible
                    powerupIconImage.color = iconColor;
                } else {
                    Debug.LogWarning($"No Image component found on Powerup Icon for button at index {targetIdx}.");
                }
            } else {
                Debug.LogWarning($"Powerup Icon not found for button at index {targetIdx}.");
            }
        }

        // Updates the icon of a loadout slot.
        public void updateAbilityIcon(int targetIdx, Sprite newImage, int colorAlpha) {
            Button currButton = loadoutButtons[targetIdx];
            currButton.transform.Find("Ability Icon").gameObject.GetComponent<Image>().sprite = newImage;
            currButton.transform.Find("Ability Icon").gameObject.GetComponent<Image>().color = Color.black;
            Color iconColor = currButton.transform.Find("Ability Icon").gameObject.GetComponent<Image>().color;
            iconColor.a = colorAlpha;
            currButton.transform.Find("Ability Icon").gameObject.GetComponent<Image>().color = iconColor;
        }

        // A connector function - this function is called from Player_Controller.cs and calls a function within UI_Loadout.
        public void updateLoadoutStatusUI(int targetIdx, bool enableAbility) {
            if (!enableAbility)
            {
                playerLoadoutUI.useCurrAbility(targetIdx);
                Debug.Log("!!!UI_Loadout found in the scene = " + playerLoadoutUI);
            }
            else
            {
                playerLoadoutUI.enableAbility(targetIdx);
            }
        }

        // swap button logic
        public void swapPowerupIcons(int firstIndex, int secondIndex) {
            Sprite firstIndexImage = powerupInventoryButtons[firstIndex].transform.Find("Powerup Icon").gameObject.GetComponent<Image>().sprite;
            Sprite secondIndexImage = powerupInventoryButtons[secondIndex].transform.Find("Powerup Icon").gameObject.GetComponent<Image>().sprite;
            deletePowerupIcon(firstIndex);
            deletePowerupIcon(secondIndex);
            updateInventoryIcon(powerupInventoryButtons, firstIndex, secondIndexImage, 255);
            updateInventoryIcon(powerupInventoryButtons, secondIndex, firstIndexImage, 255);
        }

        public void swapPowerupsInInventory(int firstIndex, int secondIndex) { // Perform the swap in the inventory
            Powerup firstIndexPowerup = (Powerup)playerPowerupInventory.items[firstIndex]; // Get the powerup at the first index
            Powerup secondIndexPowerup = (Powerup)playerPowerupInventory.items[secondIndex]; // Get the powerup at the second index
            playerPowerupInventory.swapPowerup(firstIndex, secondIndexPowerup); // Swap the powerups in the inventory
            playerPowerupInventory.swapPowerup(secondIndex, firstIndexPowerup); // Swap the powerups in the inventory
        }

        public void SetPlayerPowerupInventory(PowerupInventory powerupInventory) {
            playerPowerupInventory = powerupInventory;
        }
    
        public void swapButtonPressed() {
            Debug.Log("What's up?");
            StartCoroutine(WaitForPowerupSelection());
        }

        private IEnumerator WaitForPowerupSelection() {
            int firstIndex = playerPowerupInventoryUI.currSelected;
            if (firstIndex == -1) {
                Debug.Log("No powerup selected to start swapping!");
                yield break; // Exit if no powerup is selected
            }

            GameManager.instance.UpdateGameState(GameState.Paused); // Change the game state to 'Paused' (triggers the event)
            Debug.Log("Waiting for the player to select another powerup...");

            // Wait until the player selects a different powerup
            while (playerPowerupInventoryUI.currSelected == firstIndex || playerPowerupInventoryUI.currSelected == -1) {
                yield return null; // Wait for the next frame
            }

            int secondIndex = playerPowerupInventoryUI.currSelected;

            swapPowerupsInInventory(firstIndex, secondIndex); // Swap the powerups in the inventory

            // Perform the swap for the UI
            swapPowerupIcons(firstIndex, secondIndex);
            playerPowerupInventoryUI.selectCurrItem(firstIndex, false);
            playerPowerupInventoryUI.selectCurrItem(secondIndex, true);

            GameManager.instance.UpdateGameState(GameState.InGame); // Change the game state to 'InGame' (triggers the event)
            Debug.Log($"Swapped powerups at indices {firstIndex} and {secondIndex}.");
        }

        // A connector function - this function is called from Player_Controller.cs and calls a function within MetricBar.
        public void updateMetricBars(float currHealth) {
            healthBarUI.updateBar(currHealth);
        }
        public void LoadMenu() {
            Debug.Log("Loading menu...");
        }

        public void QuitGame() {
            Debug.Log("Quitting game...");
            Application.Quit();
        }

        // finding pausemenu and deathpanel
        GameObject GetUIElement(string elementName) {
            if (GameManager.instance.CheckGameState() == GameState.MainMenu) { // using the checkgamestate function from game manager to ensure that the game is actually running and not in menu
                Debug.LogError($"{elementName} can not be found in the MainMenu state.");
                return null;
            }
            GameObject canvas = GameObject.Find("Canvas"); // finding the canvas
            if (canvas == null) {
                Debug.LogError("Canvas not found! Ensure it's named correctly in the UI scene.");
                return null;
            }

            // Get all Transforms under the Canvas, including inactive ones
            Transform[] allTransformsInCanvas = canvas.GetComponentsInChildren<Transform>(true);
            // Loop through all found Transforms
            foreach (Transform t in allTransformsInCanvas) {
                GameObject obj = t.gameObject; // Get the GameObject from the Transform
                if (obj.name == elementName) {
                    return obj; // Return the GameObject if the name matches
                }
            }
            Debug.LogError($"{elementName} not found in any scene!");
            return null; // return null if the object is not found
        }

        // pause menu stuff. make sure to integrate it to the gamemanager afterwards
        public void Resume() {
            Debug.Log("Resuming game");
            GetUIElement("PauseMenu").SetActive(false);
            GameManager.instance.UpdateGameState(GameState.InGame); // Change the game state to 'InGame' (triggers the event)
            GameIsPaused = false;
        }
        public void Pause() {
            Debug.Log("Pausing game");
            GetUIElement("PauseMenu").SetActive(true);
            GameManager.instance.UpdateGameState(GameState.Paused); // Change the game state to 'Paused' (triggers the event)
            GameIsPaused = true;
        }

        // death stuff. make sure to integrate it to the gamemanager afterwards
        public void ToggleDeathPanel() {
            Debug.Log("Toggling death panel");
            GetUIElement("DeathPanel").SetActive(true);
            GameManager.instance.UpdateGameState(GameState.GameOver); // Change the game state to 'GameOver' (triggers the event)
        }
        public void GameOver() {
            ToggleDeathPanel();
            // add all of the other stuff that you are going to do when the player dies
        }
    }
}
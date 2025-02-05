using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIManager {
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

        public void setUpBar() {
            barSlider.minValue = minValue;
            barSlider.maxValue = maxValue;
            barSlider.value = maxValue;
            barFill.color = colorGradient.Evaluate(1f);
        }

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
        public List<Button> inventoryButtons;
        public List<Button> loadoutButtons;
        private UI_Inventory playerInventoryUI;
        private UI_Loadout playerLoadoutUI;
        public Gradient healthBarGradient;
        public Gradient energyBarGradient;
        private MetricBar healthBarUI;
        private MetricBar energyBarUI;

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

        }

        public void updateInventoryStatusUI(int targetIdx) {
            playerInventoryUI.selectCurrItem(targetIdx);
        }

        public void updateLoadoutStatusUI(int targetIdx) {
            playerLoadoutUI.useCurrAbility(targetIdx);
        }

        public void updateMetricBars(float currHealth, float currEnergy) {
            healthBarUI.updateBar(currHealth);
            energyBarUI.updateBar(currEnergy);
        }
    }
}

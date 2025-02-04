using UnityEngine;
using InteractableManager;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIManager {
    public class UI_Manager : MonoBehaviour {
        public List<Button> inventoryButtons;
        public List<Button> loadoutButtons;
        private UI_Inventory playerInventoryUI;
        private UI_Loadout playerLoadoutUI;
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
    }
}

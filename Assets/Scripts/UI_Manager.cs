using UnityEngine;
using InteractableManager;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIManager {
    public class UI_Manager : MonoBehaviour {
        public Canvas targetCanvas;
        public List<Button> inventoryButtons;
        private UI_Inventory playerInventoryUI;
        void Start() {
            playerInventoryUI = new UI_Inventory(inventoryButtons);
        }

        void Update() {

        }

        public void updateInventoryStatusUI(int targetIdx) {
            playerInventoryUI.selectCurrItem(targetIdx);
        }
    }
}

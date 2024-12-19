using UnityEngine;
using InventoryManager;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace UIManager {
    public class UI_Manager : MonoBehaviour {
        public Canvas targetCanvas;
        private List<Button> buttons;
        private UI_Inventory playerInventoryUI;
        void Start() {
            buttons = targetCanvas.GetComponentsInChildren<Button>().ToList<Button>();
            playerInventoryUI = new UI_Inventory(buttons);
        }

        void Update() {

        }

        public void updateInventoryStatusUI(int targetIdx) {
            playerInventoryUI.selectCurrItem(targetIdx);
        }
    }
}

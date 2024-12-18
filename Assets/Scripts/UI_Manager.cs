using UnityEngine;
using InventoryManager;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.RenderGraphModule;
using System.Runtime.CompilerServices;
using System.Linq;

public class UI_Inventory {
    private List<Button> buttons;
    private Dictionary<Button, int> buttonMap;

    public int currSelected = -1;

    public UI_Inventory(List<Button> buttons) {
        this.buttons = buttons;
    }

    void setUpButtonMap() {
        for (int i=0; i < buttons.Count; i++) {
            buttonMap.Add(buttons[i], i);
        }
    }

    public void selectCurrItem(int newIdx) {
        if (currSelected != -1) {
            Button prevButton = buttons[currSelected];
            Outline prevOutline = prevButton.GetComponent<Outline>();
            prevOutline.effectColor = Color.black;
            Debug.Log(prevOutline.effectColor);
        }
        if (newIdx == currSelected) {
            currSelected = -1;
        }
        if (currSelected != -1) {
            currSelected = newIdx;
            Button currButton = buttons[currSelected];
            Outline currOutline = currButton.GetComponent<Outline>();
            currOutline.effectColor = Color.yellow;
            Debug.Log(currOutline.effectColor);
        }
    }
}

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

// this is the save system script. it does not to be in any gameobject, because it just has functions that other classes can use. 
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem {
    private static Player_Save_Data saveData = new Player_Save_Data();

    public static string saveFilePath() { // we are creating a file that the save data will be stored in
        return Application.persistentDataPath + "/save" + ".save"; // this is where the save data saved for me: 'C:\Users\Adi\AppData\LocalLow\DefaultCompany\ask the ideas guy'
    }

    public static void Save() {
        handleSaveData();
        string jsonData = JsonUtility.ToJson(saveData, true); // converting the save data to a json string
        File.WriteAllText(saveFilePath(), jsonData); // writing the save data to the file in a human readable format
        Debug.Log("Game saved successfully!");
    }

    private static void handleSaveData() {
        if (GameManager.instance.Player != null) {
            Debug.Log("handleSaveData!");
            GameManager.instance.Player.GetPlayerData(ref saveData); // copying the player data to the save data variable
        } else {
            Debug.LogWarning("Player reference not found. Cannot save data.");
        }
    }

    public static void Load() {
        if (File.Exists(saveFilePath())) { // checking if the save file exists
            string jsonData = File.ReadAllText(saveFilePath()); // reading the data from the file
            saveData = JsonUtility.FromJson<Player_Save_Data>(jsonData); // making the data inside the file readable
            handleLoadData();
            Debug.Log("Game loaded successfully!");
        } else {
            Debug.LogWarning("Save file not found.");
        }
    }

    private static void handleLoadData() {
        if (GameManager.instance.Player != null) {
            Debug.Log("handleLoadData!");
            GameManager.instance.Player.LoadPlayerData(saveData); // copying the save data to the player data variable, so that it can update in game
        } else {
            Debug.LogWarning("Player reference not found. Cannot load data.");
        }
    }
}

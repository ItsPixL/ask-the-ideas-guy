using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using WeaponManager;
using PowerupManager;
using AbilityManager;
using TMPro;
using InteractableManager;
using System.Collections.Generic;

public class Spawn_Manager : MonoBehaviour {
    public static Spawn_Manager instance { get; private set; } // creating an instance for ease of use across files
    void Awake() { // creating a singleton pattern to ensure only one instance of the spawn manager is running
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // this will prevent the spawn manager from being destroyed when a new scene is loaded
        } else if (instance != this) {
            Debug.LogWarning("Duplicate Spawn_Manager detected and destroyed."); // logged as a warning so it stands out
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded_Auto;
    }
    
    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded_Auto;
    }

    // Called automatically by Unity
    void OnSceneLoaded_Auto(Scene scene, LoadSceneMode mode) {
        OnSceneLoaded(scene.name);
    }
    
    void OnSceneLoaded(string sceneName) {
        StartCoroutine(SpawnAfterDelay(sceneName)); // calling the coroutine to spawn the items after a delay
    }

    IEnumerator SpawnAfterDelay(string sceneName) {
        yield return new WaitForSeconds(0.1f); // Delay to ensure UI is ready

        if (sceneName == "SampleScene") {
            SampleSceneSpawner();
        }
        else if (sceneName == "Level 1 Design") {
            Level1Spawner();
        }
    }

    void SampleSceneSpawner() {
        TMP_Text pickupTextRef = GameObject.Find("Canvas/pickupText")?.GetComponent<TMP_Text>();
        Debug.Log("Pickup text found: " + pickupTextRef);
        Weapon testWeapon = new Sword(new List<Ability>(){new Dash(5, 10)});
        Weapon testWeapon2 = new Sword(new List<Ability>(){new Dash(5, 10)});
        GameObject testWeaponObj = testWeapon.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0));
        testWeaponObj.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        GameObject testWeaponObj1 = testWeapon2.dropItem(new Vector3(4, 1, -6), Quaternion.Euler(0, 0, 0));
        testWeaponObj1.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        Powerup testPowerup = new Fire(0);
        Powerup testPowerup1 = new Fire(0);
        Powerup testPowerup2 = new Fire(0);
        Powerup testPowerup3 = new Fire(0);
        Powerup testPowerup4 = new Poision(0);
        GameObject testPowerupObj = testPowerup.dropItem(new Vector3(-4, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerupObj.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        GameObject testPowerupObj1 = testPowerup1.dropItem(new Vector3(-5, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerupObj1.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        GameObject testPowerupObj2 = testPowerup2.dropItem(new Vector3(-6, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerupObj2.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        GameObject testPowerupObj3 = testPowerup3.dropItem(new Vector3(-7, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerupObj3.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
        GameObject testPowerupObj4 = testPowerup4.dropItem(new Vector3(-8, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerupObj4.GetComponent<Pick_Mechanic>().pickupText = pickupTextRef;
    }

    void Level1Spawner() {
        
    }
}
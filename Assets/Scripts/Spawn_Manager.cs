using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using WeaponManager;
using PowerupManager;
using AbilityManager;
using InteractableManager;
using System.Collections.Generic;
using CooldownManager;
using UIManager;
using UnityEngine.UI;

[RequireComponent(typeof(Cooldown_Manager))] // creates a new cooldown manager if one is not already present, for safety purposes.
public class Spawn_Manager : MonoBehaviour {
    public static Spawn_Manager instance { get; private set; } // creating an instance for ease of use across files
    private Cooldown_Manager cooldownManager; // declaration
    public List<Button> loadoutButtons;
    private UI_Loadout uiLoadout; // declaration
    void Awake()
    { // creating a singleton pattern to ensure only one instance of the spawn manager is running
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // this will prevent the spawn manager from being destroyed when a new scene is loaded
        }
        else if (instance != this)
        {
            Debug.LogWarning("Duplicate Spawn_Manager detected and destroyed."); // logged as a warning so it stands out
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded_Auto;
        cooldownManager = GetComponent<Cooldown_Manager>(); // assignation
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
        yield return new WaitForSeconds(0.2f); // Delay to ensure UI is ready

        if (sceneName == "SampleScene") {
            SampleSceneSpawner();
        }
        else if (sceneName == "Level 1 Design") {
            Level1Spawner();
        }
    }

    void SampleSceneSpawner() {
        // loadoutButtons = GameObject.Find("UI Manager").GetComponent<UI_Manager>().loadoutButtons; // finding the loadout panel in the scene
        // uiLoadout = new UI_Loadout(loadoutButtons, new Color(0, 0, 0, 150), new Color(255, 0, 0, 255));
        // Debug.Log("Loadout buttons found in the scene = " + loadoutButtons.Count);
        UI_Manager uiManager = GameObject.Find("UI Manager").GetComponent<UI_Manager>();
        uiLoadout = uiManager.playerLoadoutUI; // Using the existing UI_Loadout
        Debug.Log("UI_Loadout found in the scene = " + uiLoadout);
        if (cooldownManager == null)
        {
            Debug.LogError("Cooldown_Manager not found in the scene. Please ensure it is present.");
        }
        if (uiLoadout == null) {
            Debug.LogError("UI_Loadout not found in the scene. Please ensure it is present.");
        }
        Weapon testWeapon = new Sword(new List<Ability>(){new Dash(1, 10), new JabSword(cooldownManager, uiLoadout, 1, 2, 2, 0.5f)});
        Weapon testWeapon2 = new Sword(new List<Ability>(){new Dash(5, 10)});
        testWeapon.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0));
        testWeapon2.dropItem(new Vector3(4, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup = new Volcanic_Shard(0);
        Powerup testPowerup1 = new Jar_Of_Fireflies(0);
        Powerup testPowerup2 = new Void_Tether(0);
        Powerup testPowerup3 = new Frozen_Core(0);
        Powerup testPowerup4 = new Poison(0);
        testPowerup.dropItem(new Vector3(-4, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerup1.dropItem(new Vector3(-5, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerup2.dropItem(new Vector3(-6, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerup3.dropItem(new Vector3(-7, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerup4.dropItem(new Vector3(-8, 1, -6), Quaternion.Euler(0, 0, 0));
    }

    void Level1Spawner() {
        Weapon testWeapon = new Sword(new List<Ability>(){new Dash(5, 10)});
        testWeapon.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0));
        Powerup testPowerup3 = new Frozen_Core(0);
        Powerup testPowerup4 = new Poison(0);
        testPowerup3.dropItem(new Vector3(-7, 1, -6), Quaternion.Euler(0, 0, 0));
        testPowerup4.dropItem(new Vector3(-8, 1, -6), Quaternion.Euler(0, 0, 0));
    }
}
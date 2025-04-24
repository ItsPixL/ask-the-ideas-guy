using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using WeaponManager;
using PowerupManager;
using AbilityManager;
using TMPro;
using InteractableManager;
using System.Collections.Generic;
using System;

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
        testWeapon.dropItem(new Vector3(0, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        testWeapon2.dropItem(new Vector3(4, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        Powerup testPowerup = new Volcanic_Shard(0);
        Powerup testPowerup1 = new Jar_Of_Fireflies(0);
        Powerup testPowerup2 = new Void_Tether(0);
        Powerup testPowerup3 = new Frozen_Core(0);
        Powerup testPowerup4 = new Poison(0);
        testPowerup.dropItem(new Vector3(-4, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        testPowerup1.dropItem(new Vector3(-5, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        testPowerup2.dropItem(new Vector3(-6, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        testPowerup3.dropItem(new Vector3(-7, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
        testPowerup4.dropItem(new Vector3(-8, 1, -6), Quaternion.Euler(0, 0, 0), pickupTextRef);
    }

    void Level1Spawner() {
        
    }
}
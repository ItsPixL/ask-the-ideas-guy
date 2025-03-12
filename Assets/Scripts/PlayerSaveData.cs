using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable] // required for json
public struct Player_Save_Data { // we save all of the player's data in this struct. this will then get converted to json.
    public Vector3 position; // current pos
    public float health; // current health
    public List<string> inventory; // current list of items in inventory

    // Constructor for initialization
    public Player_Save_Data(Vector3 pos, float hp, List<string> inv) {
        position = pos;
        health = hp;
        inventory = inv ?? new List<string>();  // Prevents null reference
    }
}
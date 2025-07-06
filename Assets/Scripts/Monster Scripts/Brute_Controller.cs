using UnityEngine;
using MonsterManager;
using System.Collections.Generic;

public class Brute_Controller: MonoBehaviour, MonsterComponent {
    public Brute brute;
    void Start() {
        brute = (Brute)gameObject.GetComponent<Monster_Controller>().monster;
    }

    void Update()
    {
        
    }
 
}
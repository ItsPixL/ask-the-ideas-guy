using UnityEngine;
using MonsterManager;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Cinemachine;

namespace MonsterSpawnerManager {
    public class MonsterSpawner {
        public monsterType typeSpawned;
        private GameObject monsterPrefab;
        private List<List<float>> allowedSpawnPos;
        public int spawnLimit;
        public List<float> basicStats;
        public List<float> movementStats;
        public List<float> attackStats;
        public List<int> sensoryStats;
        public List<float> customStats;
        public int currMonsterCount = 0;
        
        // Initialises the monster spawner, defining the basic characteristics that control spawner behaviour.
        public MonsterSpawner(monsterType typeSpawned, GameObject monsterPrefab, List<List<float>> allowedSpawnPos, 
        int spawnLimit) {
            this.typeSpawned = typeSpawned;
            this.monsterPrefab = monsterPrefab;
            this.allowedSpawnPos = allowedSpawnPos;
            this.spawnLimit = spawnLimit;
        }

        // Initialises the common/generic stats for the monster.
        public void initCommonStats(List<float> basicStats, List<float> movementStats, List<float> attackStats, 
        List<int> sensoryStats) {
            this.basicStats = basicStats;
            this.movementStats = movementStats;
            this.attackStats = attackStats;
            this.sensoryStats = sensoryStats;
        }

        // Initialises the custom stats for the monster (varies by monster).
        public void initcustomStats(List<float> customStats) {
            this.customStats = customStats;
        }

        // Chooses a random spawn position for the monster, within a given range.
        public Vector3 chooseSpawnPos() {
            List<float> output = new List<float>();
            foreach (List<float> axisRange in allowedSpawnPos) {
                float chosenVal = (float)System.Math.Round(Random.Range(axisRange.Min(), axisRange.Max()), 2);
                output.Add(chosenVal);
            }
            return new Vector3(output[0], output[1], output[2]);
        }
 
        // Sets the common stats within the monster controller to the given values.
        public void setCommonMonsterStats(Monster_Controller newMonsterController) {
            newMonsterController.initMonster(basicStats[0], basicStats[1]);
            newMonsterController.initMonsterMovement(movementStats[0], (int)movementStats[1]);
            newMonsterController.initMonsterAttack(attackStats[0], attackStats[1]);
            newMonsterController.initMonsterSenses(sensoryStats[0], sensoryStats[1], sensoryStats[2]);
        }

        // Defines the exact value that the monster will have for each custom stat (varies by monster).
        public void setCustomMonsterStats(Monster_Controller newMonsterController) {}

        // Spawns the monster, and sets the monster stats before enabling it.
        public void spawnMonster() {
            if (currMonsterCount < spawnLimit) {
                Vector3 spawnPos = chooseSpawnPos();
                GameObject newMonster = Object.Instantiate(monsterPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                Monster_Controller newMonsterController = newMonster.GetComponent<Monster_Controller>();
                newMonsterController.initSpecificScript(typeSpawned);
                setCommonMonsterStats(newMonsterController);
                setCustomMonsterStats(newMonsterController);
                newMonsterController.setMonsterStatus(true);
                currMonsterCount += 1;
            }
        }

    }
}
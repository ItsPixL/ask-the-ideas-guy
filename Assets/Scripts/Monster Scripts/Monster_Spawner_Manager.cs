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
        public int batchSize;
        public int spawnLimit;
        public int spawnCooldown;
        public List<float> basicStats;
        public List<float> movementStats;
        public List<Vector3> dutyPath;
        public List<float> attackStats;
        public List<int> sensoryStats;
        public List<float> customStats;
        public int currMonsterCount = 0;
        public float lastSpawnedTime = float.MinValue;
        public int unattendedDeletions = 0;
        
        // Initialises the monster spawner, defining the basic characteristics that control spawner behaviour.
        public MonsterSpawner(monsterType typeSpawned, GameObject monsterPrefab, List<List<float>> allowedSpawnPos, 
        int batchSize, int spawnLimit, int spawnCooldown) {
            this.typeSpawned = typeSpawned;
            this.monsterPrefab = monsterPrefab;
            this.allowedSpawnPos = allowedSpawnPos;
            this.batchSize = batchSize;
            this.spawnLimit = spawnLimit;
            this.spawnCooldown = spawnCooldown;
        }

        // Initialises the common/generic stats for the monster.
        public void initCommonStats(List<float> basicStats, List<float> movementStats, List<Vector3> dutyPath, List<float> attackStats, 
        List<int> sensoryStats) {
            this.basicStats = basicStats;
            this.movementStats = movementStats;
            this.dutyPath = dutyPath;
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
            newMonsterController.initMonster(basicStats[0], basicStats[1], this);
            newMonsterController.initMonsterMovement(movementStats[0], (int)movementStats[1], dutyPath);
            newMonsterController.initMonsterAttack(attackStats[0], attackStats[1]);
            newMonsterController.initMonsterSenses(sensoryStats[0], sensoryStats[1], sensoryStats[2]);
        }

        // Defines the exact value that the monster will have for each custom stat (varies by monster).
        public void setCustomMonsterStats(Monster_Controller newMonsterController) {}

        // Spawns the monster, and sets the monster stats before enabling it.
        public int spawnMonster(bool isNatural, int collectiveLimit, int unnaturalAmount=0) {
            int amountToSpawn = 0;
            int layer = LayerMask.NameToLayer("Enemy");
            if (currMonsterCount < spawnLimit)
            {
                if (!isNatural || Time.time - lastSpawnedTime >= spawnCooldown)
                {
                    if (isNatural)
                    {
                        amountToSpawn = new List<int> { spawnLimit - currMonsterCount, collectiveLimit, batchSize }.Min();
                    }
                    else
                    {
                        amountToSpawn = new List<int> { spawnLimit - currMonsterCount, collectiveLimit, unnaturalAmount }.Min();
                    }
                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        Vector3 spawnPos = chooseSpawnPos();
                        GameObject newMonster = Object.Instantiate(monsterPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                        newMonster.layer = layer; // setting the layer to "Enemy"
                        Monster_Controller newMonsterController = newMonster.GetComponent<Monster_Controller>();
                        newMonsterController.initSpecificScript(typeSpawned);
                        setCommonMonsterStats(newMonsterController);
                        setCustomMonsterStats(newMonsterController);
                        newMonsterController.setMonsterStatus(true);
                        currMonsterCount += 1;
                    }
                    lastSpawnedTime = Time.time;
                }
            }
            return amountToSpawn;
        }

        // When monster is defeated, if all monsters were previously alive, resume spawn cooldown only when 1 is dead.
        public void onMonsterDefeat() {
            if (currMonsterCount == spawnLimit) {
                lastSpawnedTime = Time.time;
            }
            currMonsterCount -= 1;
            unattendedDeletions += 1;
        }
    }

    public class MonsterSpawnerConnector {
        public List<MonsterSpawner> monsterSpawners;
        public int totalSpawnLimit;
        private int currSpawned = 0;

        public MonsterSpawnerConnector(List<MonsterSpawner> monsterSpawners, int totalSpawnLimit) {
            this.monsterSpawners = monsterSpawners;
            this.totalSpawnLimit = totalSpawnLimit;
        }

        // Spawns monsters from each spawner, and keeps track of the collective number of monsters spawned.
        public void spawnMonsters() {
            foreach (MonsterSpawner monsterSpawner in monsterSpawners) {
                currSpawned += monsterSpawner.spawnMonster(true, totalSpawnLimit-currSpawned);
                if (monsterSpawner.unattendedDeletions > 0) {
                    currSpawned -= monsterSpawner.unattendedDeletions;
                    monsterSpawner.unattendedDeletions = 0;
                }
            }
        }

        // Spawns monsters from a given spawner in a forced/unnatural manner (i.e. not following spawn cooldowns).
        public void unnaturalSpawning(MonsterSpawner monsterSpawner, int spawnTarget) {
            if (monsterSpawners.Contains(monsterSpawner)) {
                currSpawned += monsterSpawner.spawnMonster(false, totalSpawnLimit-currSpawned, spawnTarget);
            }
        }

    }
}
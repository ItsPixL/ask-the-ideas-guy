using UnityEngine;
using MonsterManager;
using System.Collections.Generic;
using System.Linq;

namespace MonsterSpawnerManager {
    public class MonsterSpawner {
        public monsterType typeSpawned;
        private GameObject monsterPrefab;
        private List<List<float>> allowedSpawnPos;
        public int spawnLimit;
        public List<List<float>> basicStatsRange;
        private List<int> basicStatRoundings; 
        public List<List<float>> movementStatsRange;
        private List<int> movementStatRoundings; 
        public List<List<float>> attackStatsRange;
        private List<int> attackStatRoundings; 
        public List<List<int>> sensoryStatsRange;
        public List<List<float>> customStatsRange;
        private List<int> customStatRoundings;
        public int currMonsterCount = 0;
        
        // Initialises the monster spawner, defining the basic characteristics that control spawner behaviour.
        public MonsterSpawner(monsterType typeSpawned, GameObject monsterPrefab, List<List<float>> allowedSpawnPos, 
        int spawnLimit) {
            this.typeSpawned = typeSpawned;
            this.monsterPrefab = monsterPrefab;
            this.allowedSpawnPos = allowedSpawnPos;
            this.spawnLimit = spawnLimit;
        }

        // Initialises the range of values that the basic stats can be within for the monster.
        public void initBasicStats(List<List<float>> basicStatsRange, List<int> basicStatRoundings) {
            this.basicStatsRange = basicStatsRange;
            this.basicStatRoundings = basicStatRoundings;
        }

        // Initialises the range of values that the movement related stats can be within for the monster.
        public void initMovementStats(List<List<float>> movementStatsRange, List<int> movementStatRoundings) {
            this.movementStatsRange = movementStatsRange;
            this.movementStatRoundings = movementStatRoundings;
        }

        // Initialises the range of values that the attack related stats can be within for the monster.
        public void initAttackStats(List<List<float>> attackStatsRange, List<int> attackStatRoundings) {
            this.attackStatsRange = attackStatsRange;
            this.attackStatRoundings = attackStatRoundings;
        }

        // Initialises the range of values that the sensory related stats can be within for the monster.
        public void initSensoryStats(List<List<int>> sensoryStatsRange) {
            this.sensoryStatsRange = sensoryStatsRange;
        }

        // Initialises the range of values that the custom stats can be within for the monster (varies by monster).
        public void initcustomStatsRange(List<List<float>> customStatsRange, List<int> customStatRoundings) {
            this.customStatsRange = customStatsRange;
            this.customStatRoundings = customStatRoundings;
        }

        // For each List<int> within a larger list, choose a random integer between the two numbers in that List<int>.
        public List<int> chooseRandomInts(List<List<int>> initialInts) {
            List<int> output = new List<int>();
            foreach (List<int> intRange in initialInts) {
                int chosenInt = Random.Range(intRange.Min(), intRange.Max()+1);
                output.Add(chosenInt);
            }
            return output;
        }

        // For each List<float> within a larger list, choose a random float between the two numbers in that List<float>.
        // Rounding is done to each float value to some d.p. This is for memory and performance optimisation purposes.
        public List<float> chooseRandomFloats(List<List<float>> initialFloats, List<int> decimalPlaces) {
            List<float> output = new List<float>();
            int idx = 0;
            foreach (List<float> floatRange in initialFloats) {
                float chosenFloat = (float)System.Math.Round(Random.Range(floatRange.Min(), floatRange.Max()), decimalPlaces[idx]);
                output.Add(chosenFloat);
                idx += 1;
            }
            return output;
        }

        // Defines the exact value that the monster will have for each common stat.
        public void setCommonMonsterStats(GameObject newMonster) {
            Monster_Controller newMonsterController = newMonster.GetComponent<Monster_Controller>();
            List<float> basicStats = chooseRandomFloats(basicStatsRange, basicStatRoundings);
            List<float> movementStats = chooseRandomFloats(movementStatsRange, movementStatRoundings);
            List<float> attackStats = chooseRandomFloats(attackStatsRange, attackStatRoundings);
            List<int> sensoryStats = chooseRandomInts(sensoryStatsRange);
            newMonsterController.initSpecificScript(typeSpawned);
            newMonsterController.initMonster(basicStats[0], basicStats[1]);
            newMonsterController.initMonsterMovement(movementStats[0], (int)movementStats[1]);
            newMonsterController.initMonsterAttack(attackStats[0], attackStats[1]);
            newMonsterController.initMonsterSenses(sensoryStats[0], sensoryStats[1], sensoryStats[2]);
        }

        // Spawns the monster, and sets the monster stats before enabling it.
        public void spawnMonster() {
            List<float> spawnPosFloat = chooseRandomFloats(allowedSpawnPos, new List<int>{2, 2, 2});
            Vector3 spawnPos = new Vector3(spawnPosFloat[0], spawnPosFloat[1], spawnPosFloat[2]);
            GameObject newMonster = Object.Instantiate(monsterPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            setCommonMonsterStats(newMonster);
        }

    }
}
using UnityEngine;
using MonsterManager;
using MonsterSpawnerManager;
using System.Collections.Generic;

public class Testing_Monster_Spawner: MonoBehaviour {
    public List<string> nameRegistry;
    public List<GameObject> prefabRegistry;
    private MonsterSpawner testSpawner1;
    private MonsterSpawner testSpawner2;
    private MonsterSpawnerConnector testConnector1;

    void Start() {
        createTestMonsters();
    }

    void Update() {
        testConnector1.spawnMonsters();
    }

    private GameObject convertToPrefab(string monsterVarient) {
        int targetIdx = nameRegistry.IndexOf(monsterVarient);
        if (targetIdx == -1) {
            return prefabRegistry[0];
        }
        return prefabRegistry[targetIdx];
    } 

    private void createTestMonsters() {
        List<List<float>> allowedSpawnPos = new List<List<float>>{
            new List<float>{-8f, 5f},
            new List<float>{1f, 1f},
            new List<float>{-9.1f, -5f}
        };
        List<Vector3> dutyPath = new List<Vector3>{
            new Vector3(-8, 1, -5),
            new Vector3(-8, 1, -9),
            new Vector3(5, 1, -9),
            new Vector3(5, 1, -5)
        };
        List<float> basicStats = new List<float>{10, 5};
        List<float> movementStats = new List<float>{5, 180};
        List<float> attackStats = new List<float>{1.5f, 1.5f};
        List<int> sensoryStats = new List<int>{10, 5, 160};
        testSpawner1 = new MonsterSpawner(monsterType.Brute, convertToPrefab("Brute"), allowedSpawnPos, 1, 1, 15);
        testSpawner2 = new MonsterSpawner(monsterType.Brute, convertToPrefab("Brute"), allowedSpawnPos, 2, 4, 20);
        testSpawner1.initCommonStats(basicStats, movementStats, dutyPath, attackStats, sensoryStats);
        testSpawner2.initCommonStats(basicStats, movementStats, dutyPath, attackStats, sensoryStats);
        testConnector1 = new MonsterSpawnerConnector(new List<MonsterSpawner>{testSpawner1}, 5);
    }
}
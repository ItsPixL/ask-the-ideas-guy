using UnityEngine;
using MonsterManager;
using MonsterSpawnerManager;
using System.Collections.Generic;
using Unity.Cinemachine;

public class Testing_Monster_Spawner: MonoBehaviour {
    public List<string> nameRegistry;
    public List<GameObject> prefabRegistry;
    private MonsterSpawner testSpawner1;

    void Start() {
        createTestMonster();
    }

    void Update() {
        testSpawner1.spawnMonster();
    }

    private GameObject convertToPrefab(string monsterVarient) {
        int targetIdx = nameRegistry.IndexOf(monsterVarient);
        if (targetIdx == -1) {
            return prefabRegistry[0];
        }
        return prefabRegistry[targetIdx];
    } 

    private void createTestMonster() {
        List<List<float>> allowedSpawnPos = new List<List<float>>{
            new List<float>{-8f, 5f},
            new List<float>{1f, 1f},
            new List<float>{-9.1f, -5f}
        };
        List<float> basicStats = new List<float>{10, 5};
        List<float> movementStats = new List<float>{5, 180};
        List<float> attackStats = new List<float>{1.5f, 1.5f};
        List<int> sensoryStats = new List<int>{10, 5, 160};
        testSpawner1 = new MonsterSpawner(monsterType.Brute, convertToPrefab("Brute"), allowedSpawnPos, 1);
        testSpawner1.initCommonStats(basicStats, movementStats, attackStats, sensoryStats);
    }
}
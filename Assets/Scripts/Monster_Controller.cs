using UnityEngine;
using MonsterManager;

public class Monster_Controller : MonoBehaviour {
    private LineRenderer[] visionLines;
    Monster entity = new Monster(5f, 10, 15, 160); 

    // The function below is for testing purposes only. It will be removed when all of the code is finalised.
    void OnDrawGizmos() {
        int targetDistance = entity.sightRange;
        if (entity.monster == null) return;

        Vector2 monsterForward2D = new Vector2(entity.monster.transform.forward.x, entity.monster.transform.forward.z).normalized;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -entity.fieldOfView / 2) * monsterForward2D;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, entity.fieldOfView / 2) * monsterForward2D;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(leftBoundary.x, 0, leftBoundary.y) * targetDistance);
        Gizmos.DrawRay(entity.monster.transform.position, new Vector3(rightBoundary.x, 0, rightBoundary.y) * targetDistance);
    }

    void Start() {
        entity.initGameObjects(gameObject, GameObject.FindWithTag("Player"));
        visionLines = GetComponentsInChildren<LineRenderer>();
        entity.setVisionScript(visionLines);
        
    }

    void Update() {
        entity.checkForPlayer();
        entity.updateVisionLines();
    }
}

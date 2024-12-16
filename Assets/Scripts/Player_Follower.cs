using UnityEngine;

public class Player_Follower : MonoBehaviour {
    public Transform player;
    public Vector3 offset;

    // Update is called once per frame, after all Update() functions.
    void LateUpdate() {
        transform.position = player.position + offset;
    }
}

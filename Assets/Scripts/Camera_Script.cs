using UnityEngine;
using System.Collections.Generic;

public class Camera_Script : MonoBehaviour {
    private GameObject player;
    private HashSet<ObjectFader> currentlyFading = new HashSet<ObjectFader>();

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (player == null) return;

        Vector3 dir = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        float distance = dir.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(ray, distance);
        HashSet<ObjectFader> newFaders = new HashSet<ObjectFader>();

        foreach (RaycastHit hit in hits) {
            if (hit.collider == null || hit.collider.gameObject == player)
                continue;

            ObjectFader fader = hit.collider.GetComponent<ObjectFader>();
            if (fader != null) {
                fader.doFade = true;
                newFaders.Add(fader);
            }
        }

        // Reset fade on objects no longer obstructing
        foreach (ObjectFader fader in currentlyFading) {
            if (!newFaders.Contains(fader)) {
                fader.doFade = false;
            }
        }

        currentlyFading = newFaders;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Update_Closest_Item : MonoBehaviour
{
    [HideInInspector] public List<GameObject> objectsOfConcern = new List<GameObject>();
    [HideInInspector] public GameObject closestObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (objectsOfConcern.Count > 0) {
            float currMin = float.PositiveInfinity;
            GameObject currClosestObject = null;
            foreach (GameObject currObject in objectsOfConcern) {
                float currDistance = Vector3.Distance(gameObject.transform.position, currObject.transform.position);
                if (currMin > currDistance) {
                    currMin = currDistance;
                    currClosestObject = currObject;
                }
            }
            closestObject = currClosestObject;
        }
    }
}

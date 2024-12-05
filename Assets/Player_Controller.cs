using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        if (Input.GetKey("w")) { // forwards
            rb.AddForce(0, 0, force * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey("s")) { // backwards
            rb.AddForce(0, 0, -force * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey("a")) { // left
            rb.AddForce(-force * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        else if (Input.GetKey("d")) { // right
            rb.AddForce(force * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
    }
}

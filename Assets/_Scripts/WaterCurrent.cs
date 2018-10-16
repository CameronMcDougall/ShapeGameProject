using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script for moving the player with a water current. 
 * 
 * Author: Kristian Hansen
 */
public class WaterCurrent : MonoBehaviour {

    [SerializeField]
    public float velocity; // magnitude of speed of objects
    [SerializeField]
    public float direction; // angle (0-360) at which the water current moves

    private float FACTOR = 100f;

	// Use this for initialization
	void Start () {
        Mathf.Clamp(direction, 0f, 360f);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entered water");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.transform.position += new Vector3(1.0f, 0.0f, 0.0f) * (velocity / FACTOR);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("exited water");
        }
    }
}

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
    // relative to the orientation of the block.

    private readonly float FACTOR = 100f;

    private float delta;
    private Vector3 movement; 

	// Use this for initialization
	void Start () {
        Mathf.Clamp(direction, 0f, 360f);
        direction *= Mathf.Deg2Rad; // direction is now radians for trig maths
        delta = velocity / FACTOR;
        // TODO: Clean up this maths here to use unitys vectors rather than raw maths\
        float dx = delta * Mathf.Sin(direction);
        float dy = delta * Mathf.Cos(direction);

        // only need to calculate once as its a constant movement. Saves calculation space
        movement = new Vector3(dx, 0.0f, dy);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            // the ball is in the water so get rid of prev velocity. 
            collision.collider.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            var trans = collision.collider.gameObject.transform;
            trans.position += movement;
        }
    }
}

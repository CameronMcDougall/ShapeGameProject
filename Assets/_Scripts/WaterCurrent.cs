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

	// Use this for initialization
	void Start () {
        Mathf.Clamp(direction, 0f, 360f);
	}

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}

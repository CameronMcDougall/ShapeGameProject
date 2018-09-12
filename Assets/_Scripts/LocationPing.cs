using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationPing : MonoBehaviour {
	/*
	 * Utility script that reports attached objects location to console on a timed delay
	 * Sean Kells; kellssean@myvuw.ac.nz
	 * 07/08/18
	 */

	//the time value in seconds for the delay
	public float delayLength;
	//the current delay timer
	private float delay = 0;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
		if (delay <= 0) {
			delay = delayLength;
			Debug.Log ("Name: " + transform.position.ToString());
		} else {
			delay = delay - Time.deltaTime;
		}
	}
}

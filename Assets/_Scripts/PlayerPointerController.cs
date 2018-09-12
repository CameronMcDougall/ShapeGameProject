using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointerController : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float xInput = Input.GetAxis ("Mouse X");
		transform.position = player.transform.position;
		transform.RotateAround (transform.position, Vector3.up, xInput);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	/* 
	 * Camera controller for Shape game; MDDN243/COMP313 course A2
	 * Sean Kells; kellssean@myvuw.ac.nz
	 * 31/07/18
	 */

	//camera pointer for camera to follow
	public GameObject cameraPointer;

	//player reference
	public GameObject player;

	//initial follow constraints
	public float followDistance;
	public float followHeight;

	//upper and lower angle bounds for rotation about Vector.right
	public float yAngleLowerBound;
	public float yAngleUpperBound;

	//heading and tilt for camera direction
	private float heading = 0;
	private float tilt = 0;

	public float mouseSensitivityX;
	public float mouseSensitivityY;

	private Vector3 offset;
	private float camDist = 0;

	//tracks camera inversion, toggle inversion in editor
	public bool invert;

	// Use this for initialization
	void Start () {
		if (mouseSensitivityX < 1)
			mouseSensitivityX = 1;
		if (mouseSensitivityY < 1)
			mouseSensitivityY = 1;

		transform.Translate (new Vector3(0.0f, followHeight, -followDistance));
		transform.LookAt (cameraPointer.transform.position);
		camDist = Mathf.Sqrt (Mathf.Pow (followHeight, 2f) + Mathf.Pow (followDistance, 2f));
		tilt = transform.rotation.eulerAngles.x;

		offset = transform.position - player.transform.position;
		camDist = offset.magnitude;
	}
		
	void Update () {
		heading += Input.GetAxis ("Mouse X") * mouseSensitivityX;
		tilt += Input.GetAxis ("Mouse Y") * mouseSensitivityY;

		if (tilt < -30)
			tilt = -30;
		if (tilt > 60)
			tilt = 60;

		transform.rotation = Quaternion.Euler (tilt, heading, 0f);

		transform.position = player.transform.position - transform.forward * camDist;
	}
}

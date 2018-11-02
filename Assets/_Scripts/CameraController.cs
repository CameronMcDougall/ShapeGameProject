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

    [SerializeField]
	public float mouseSensitivityX;
    [SerializeField]
	public float mouseSensitivityY;
    [SerializeField]
    public float zoomSensitivity;

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
		
	void FixedUpdate () {
        float x = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float y = Input.GetAxis("Mouse Y") * mouseSensitivityY;
        
        if (invert)
        {
            y = -y;
        }

        transform.rotation *= Quaternion.Euler(new Vector3(-y, x));
        if (transform.rotation.eulerAngles.x > 90f) {
            //camera does not go under plane
            transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }

        transform.Rotate(0, 0, -transform.eulerAngles.z);
        var pointer = player.transform;

		Vector3 desiredPosition = pointer.position - transform.forward * camDist;
        transform.position = desiredPosition;

        RaycastHit[] hits = Physics.RaycastAll(desiredPosition, pointer.position - desiredPosition, (pointer.position - desiredPosition).magnitude);
		if (hits.Length != 1 /*&& hits.collider.CompareTag("Spawn")==false*/)
        {
            if (hits.Length == 2 && hits[0].collider.CompareTag("Respawn"))
            {
                return;
            }
            int index = hits.Length - 2;
			RaycastHit end = hits[index];
		    while (end.collider.CompareTag("Respawn"))
            {
                if (index - 1 < 0)
                {
                    return;
                }
                end = hits[index - 1];
                index = index - 1;
            }
			//RaycastHit end = hits[0];
            transform.position = (end.point - pointer.position) * 0.8f + pointer.position;

        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderPlayerTEST : MonoBehaviour {

	private Rigidbody rb;

	// min and max sides for player object
	private int minSides;
	private int maxSides;

	// player current sides
	private int sides;

	// camera reference for movement vectors
	public GameObject playerPointer;

	// player spawn point
	public GameObject spawn;

	// editor set fixed speed variable
	public float speed;

	// jump value for editor
	public float jumpAmount;

	// boost value
	public float boost;

	// tracks if player is grounded
	private bool grounded = true;

	// tracks total charge amount
	private float charge = 0;

	// tracks morph state
	private float mor;

	Vector3 actionCl = new Vector3 (0.0f, 0.0f, 0.0f);

	void Start () {
		rb = GetComponent<Rigidbody> ();
		mor = 1;
	}
	
	// Update is called once per frame
	void Update () {
		// testie
		if ((Input.GetAxis("Action")>0) && charge < 50) {
			charge = charge + 1;
		}
		if (Input.GetKeyUp(KeyCode.Space) && grounded && charge >= 50) {
			actionCl = playerPointer.transform.forward * boost;
			Debug.Log (actionCl.ToString());
			charge = 0;
		}
		//print ("Charging:" + charge);
	}


	void LateUpdate () {
		//Debug.Log ("X: " + rb.velocity.x + " Y: " + rb.velocity.y + " Z: " + rb.velocity.z);
		float fwdInput = Input.GetAxis ("Vertical");
		float sideInput = Input.GetAxis ("Horizontal");
		Vector3 camForward = playerPointer.transform.forward * fwdInput;
		Vector3 camRight = playerPointer.transform.right * sideInput;

		Vector3 movement = camForward + camRight + actionCl;
		rb.AddForce (movement * speed);




	}

	void OnTriggerEnter(Collider other) {
		playerInBoundsCheck (other);
	}

	void playerInBoundsCheck (Collider other) {
		if (other.CompareTag ("KillBox")) {
			transform.position = spawn.transform.position;
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			rb.angularVelocity = new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.CompareTag ("Ground")) {
			grounded = true;
		}
	}

//	Vector3 actionLogic(){
//		Vector3 camUpward = new Vector3 (0.0f, 0.0f, 0.0f);
//		if(grounded & (Input.GetKeyDown(KeyCode.Space) = true)) {
//			grounded = false;
//			charge = charge + 1;
//			if (charge > 50 & (Input.GetKeyUp(KeyCode.Space) = true)) {
//				camUpward = playerPointer.transform.forward * (float)Input.GetAxis ("Action") * boost;
//			}
	// return camUpward;
		}

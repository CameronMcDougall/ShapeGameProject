using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentEffect : MonoBehaviour {

	private Rigidbody rb;
	private Vector3 force;
	private Transform parentTrans;
	public float ventPower;

	// Use this for initialization
	void Start () {
		rb = null;
		force = new Vector3 (0.0f, 0.0f, 0.0f);
		parentTrans = GetComponentInParent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {
		if (other.CompareTag("Player")) {
			rb = other.GetComponentInParent<Rigidbody>();
			force = calcForce ();
			rb.AddForce (force);
		}
	}

	Vector3 calcForce () {
		Transform otherTrans = rb.GetComponentInParent<Transform> ();
		Vector3 disp = parentTrans.position - otherTrans.position;
		Debug.Log (disp.ToString());
		float power = 11.0f; //- Mathf.Clamp(disp.magnitude, 1.0f, 2.0f);
		float yThrust = 10f * power;
		return new Vector3 (0.0f, yThrust, 0.0f);
	}
}

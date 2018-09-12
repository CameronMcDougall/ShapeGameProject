using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public Camera mainCamera;
	public Camera flyingCamera;
	private AudioListener mcAL;
	private AudioListener fcAL;
	private bool trigger = false;


	// Use this for initialization
	void Start () {
		mcAL = mainCamera.GetComponent<AudioListener>();
		mainCamera.enabled = true;
		mcAL.enabled = true;
		fcAL = flyingCamera.GetComponent<AudioListener>();
		flyingCamera.enabled = false;
		fcAL.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Camera") > 0 && trigger == false) {
			trigger = true;
			mainCamera.enabled = !mainCamera.enabled;
			mcAL.enabled = !mcAL.enabled;
			flyingCamera.enabled = !flyingCamera.enabled;
			fcAL.enabled = !fcAL.enabled;
		} else if (Input.GetAxis ("Camera") <= 0 && trigger == true) {
			trigger = false;
		}
	}
}

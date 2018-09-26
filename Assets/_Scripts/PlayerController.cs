using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	/* 
	 * Player controller for Shape game; MDDN243/COMP313 course A2
	 * Sean Kells; kellssean@myvuw.ac.nz
	 * 29/07/18
	 */

	private Rigidbody rb;

	// min and max sides for player object
	private int minSides;
	private int maxSides;

	// player current sides
	private int sides;

	// camera reference for movement vectors
	public GameObject playerPointer;
	public GameObject cameraPointer;

	public GameObject player;

	public Camera cam;

	// player spawn point
	public GameObject spawn;

	// editor set fixed speed variable
	public float speed;

	//
	public int vexRatio = 5;

	// 
	public float glideFallSpeed;
	public float heavyFallSpeed;

	// boost value
	public float boost;

	// tracks total charge amount
	private float charge = 0;

	private float shrinkDelay = 0;

    // Action value for editor
    public float ActionAmount;

	// tracks if player is grounded
	private bool grounded = true;
	private bool shrunk = false;

	// tracks morph state
	public float mor;

	public Mesh sphere;
	public Mesh cube;
	public Mesh cylinder;

	public AudioSource jsound;
	public AudioSource csound;
	public AudioSource msound;
	public AudioSource lsound;

	private int vexCycler = 0;

	List<GameObject> currentCollisions = new List<GameObject>();
	
	Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);

	void Start () {
		rb = GetComponent<Rigidbody> ();
		mor = 1;
	}

	void Update () {
		float deltaT = Time.deltaTime;
		shrinkDelay -= deltaT;

		//Debug.Log (1 / 4);

		rayCastGround(getLowestVertex());
		
		updateShape();

		movementLogic();

		actionLogic();

		//Debug.Log ("Grounded: " + grounded);

	}

	void movementLogic(){
		float fwdInput;
		float sideInput;
		if (mor == 2) {
			fwdInput = Input.GetAxis ("Vertical");
			sideInput = 0f;
			transform.RotateAround (playerPointer.transform.position, playerPointer.transform.up, Input.GetAxis ("Horizontal"));
		} else {
			fwdInput = Input.GetAxis ("Vertical");
			sideInput = Input.GetAxis ("Horizontal");
		}
		Vector3 camForward2 = new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z) * fwdInput;
		Vector3 camRight2 = new Vector3(cam.transform.right.x, 0.0f, cam.transform.right.z) * sideInput;
		movement = camForward2 + camRight2;

		rb.AddForce (movement * speed);
	}

	void actionLogic(){
		// Cylinder movement input action
		if(mor==2){
			if ((Input.GetAxis ("Action") > 0) && grounded && charge < 50) {
				charge = charge + 1;
				if (!csound.isPlaying) {
					csound.Play ();
				}
			}
			if ((Input.GetAxis ("Action") > 0) && grounded && charge >= 50) {
				charge = charge + 1;
				if (!msound.isPlaying) {
					msound.Play();
				}
			}
			if ((Input.GetAxis("Action") == 0) && grounded && charge >= 50) {
				msound.Stop();
				lsound.Play();
				Vector3 actionCl = (playerPointer.transform.forward * boost) + playerPointer.transform.up * 40;
				rb.AddForce(actionCl);
				charge = 0;
			}
		}else if(mor==3){
			Vector3 down = new Vector3 (0.0f, -1f, 0.0f);
			if (shrunk) {
				if (grounded) {
					shrink (false);
				} else {
					if (Input.GetAxis ("Action") > 0 && shrinkDelay <= 0) {
						shrink (false);
						shrinkDelay = 0.4f;
						down = down * glideFallSpeed;
					}
				}
			} else {
				if (!grounded) {
					if (Input.GetAxis ("Action") > 0 && shrinkDelay <= 0) {
						shrink (true);
						shrinkDelay = 0.4f;
					} else {
						down = down * heavyFallSpeed;
					}
				}
			}
			rb.AddForce (down);
		}else{
			Vector3 camUpward = new Vector3 (0.0f, 0.0f, 0.0f);
			if(grounded & (Input.GetAxis ("Action") > 0)) {
				camUpward = playerPointer.transform.up * (float)Input.GetAxis ("Action") * ActionAmount;
				grounded = false;

				if (!jsound.isPlaying) {
					jsound.Play();
				}
			}
			rb.AddForce (camUpward * speed);
		}
	}

	void shrink (bool b) {
		if (b) {
			if (!shrunk) {
				transform.localScale = transform.localScale + new Vector3 (-0.7f, -0.7f, -0.7f);
				rb.useGravity = false;
				shrunk = true;
				rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
			}
		} else {
			if (shrunk) {
				transform.localScale = transform.localScale + new Vector3 (0.7f, 0.7f, 0.7f);
				rb.useGravity = true;
				shrunk = false;
			}
		}
	}

	void updateShape() {
		//Explosion effect for morphing
		var exp = GetComponent<ParticleSystem> ();
		//currentCollisions = new List<GameObject>();

		//Morphing activators/deactivators
        // LHansen - Changed cylinder key to 2 and cube to 3. 27/09
		//Sphere to Cylinder
			if (Input.GetKeyDown(KeyCode.Alpha2) && mor==1) {
				exp.Play ();
				GetComponent<MeshCollider>().enabled = true;
				GetComponent<SphereCollider>().enabled = false;
				GetComponent<MeshFilter>().mesh = cylinder;

				mor = 2;
			}
        //Sphere to box
            if (Input.GetKeyDown(KeyCode.Alpha3) && mor == 1)
            {
                exp.Play();
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<MeshFilter>().mesh = cube;
                mor = 3;
            }
		//Cylinder to Sphere
			if (Input.GetKeyDown(KeyCode.Alpha1) && mor==2) {
				
				exp.Play ();
				GetComponent<SphereCollider>().enabled = true;
				GetComponent<MeshCollider>().enabled = false;
				GetComponent<MeshFilter>().mesh = sphere;

				mor = 1;
			}
		//Cylinder to Box
			if (Input.GetKeyDown(KeyCode.Alpha3) && mor==2) {
				exp.Play ();
				GetComponent<BoxCollider>().enabled = true;
				GetComponent<MeshCollider>().enabled = false;
				GetComponent<MeshFilter>().mesh = cube;

				mor = 3;
			}
        //Box to Sphere
            if (Input.GetKeyDown(KeyCode.Alpha1) && mor == 3)
            {
                if (shrunk)
                {
                    shrink(false);
                }
                exp.Play();
                GetComponent<SphereCollider>().enabled = true;
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<MeshFilter>().mesh = sphere;

                mor = 1;
            }
        //Box to Cylinder
            if (Input.GetKeyDown(KeyCode.Alpha2) && mor == 3)
            {
                if (shrunk)
                {
                    shrink(false);
                }
                exp.Play();
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<MeshFilter>().mesh = cylinder;

                mor = 2;
            }
	}

	Vector3 getLowestVertex() {
		Vector3 lowest = new Vector3(0f, Mathf.Infinity, 0f);
		Mesh m = player.GetComponent<MeshFilter> ().mesh;
		Vector3[] vecs = m.vertices;
		int inc = 12;
		if (mor == 2)
			inc = 1;
		if (mor == 3)
			inc = 3;
		if (vexCycler > inc)
			vexCycler = 0;
		for(int i = vexCycler; i < vecs.Length; i += inc) {
			Debug.Log (i);
			Vector3 temp = player.transform.TransformVector (vecs[i]) + player.transform.position;

			if (temp.y < lowest.y)
				lowest = temp;
			
		}
		vexCycler ++;
		return lowest;
	}

	void rayCastGround(Vector3 lowest) {
		int layerMask = LayerMask.GetMask ("Default");
		RaycastHit hit;
		Ray ray = new Ray (lowest + new Vector3(0.0f, 0.01f, 0.0f), -playerPointer.transform.up);
		Debug.Log (ray.origin.y);
		if(Physics.Raycast(ray, out hit, 0.09f, layerMask, QueryTriggerInteraction.Ignore)){
			grounded = true;
			if(hit.collider.CompareTag("Ground")){
				Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.yellow, 3f);
			} else {
				Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.red, 3f);
				grounded = false;
			}
		}else{
			grounded = false;
			Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.white, 3f);
		}
	}


	void LateUpdate () {
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

	//void OnCollisionEnter(Collision collision){
		//if(collision.gameObject.CompareTag ("Ground")) {
			//grounded = true;
		//}
	//}

	void OnCollisionEnter (Collision col) {
	}

	void OnCollisionExit (Collision col) {
	}




}




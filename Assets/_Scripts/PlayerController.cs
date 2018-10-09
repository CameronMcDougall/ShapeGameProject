using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /* 
     * Player controller for Shape game; MDDN243/COMP313 course A2
     * Sean Kells; kellssean@myvuw.ac.nz
     * 29/07/18
     */


    /*
     * Modified and optimized by Kristian Hansen (hansenkris@myvuw.ac.nz)
     * as of 03/10/18
     */

    // representation of current shape. better than a single arbitrary integer
    protected enum ShapeVar { SPHERE, CYLINDER, CUBE, NONE };

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

    [SerializeField]
    public float breakingMomentum;

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
    protected ShapeVar mor;

    public Mesh sphere;
    public Mesh cube;
    public Mesh cylinder;

    public AudioSource jsound;
    public AudioSource csound;
    public AudioSource msound;
    public AudioSource lsound;

    // Slider to visually show charging cylinder.     public Slider chargeSlider;     public Image cs_FillImage;                           public Color cs_FullChargeColor = Color.green;       public Color cs_ZeroChargeColor = Color.red; 

    // Displays the current attempt the player is on.
    public Text attemptText;     private int attemptNo = 1;

    private int vexCycler = 0;

    List<GameObject> currentCollisions = new List<GameObject>();

    Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
    // temp collider for collisions with moving platforms. needed for correct scaling
    private GameObject colliderTemp = null;
    // the object the player is colliding with to avoid collisions twice
    private GameObject collidingWith = null;
    private bool hasPaused;
    void Start()
    {
        hasPaused = false;
        rb = GetComponent<Rigidbody>();
        mor = ShapeVar.SPHERE;
        // Set the text for the current attempt.         SetAttemptText();         // Set the slider to be invisible at the start (so it only shows for the cylinder)         chargeSlider.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            this.hasPaused = !this.hasPaused;
        }
            
        if (!this.hasPaused)
        {
            Time.timeScale = 1.0f;
            this.updateGame();
        }
        else {
            Time.timeScale = 0.0f;

        }   
    }
    void updateGame() {
        float deltaT = Time.deltaTime;
        shrinkDelay -= deltaT;
        rayCastGround(getLowestVertex());
        updateShape();
        movementLogic();
        actionLogic();
    }
    void movementLogic()
    {
        float fwdInput;
        float sideInput;
        if (mor == ShapeVar.CYLINDER)
        {
            fwdInput = Input.GetAxis("Vertical");
            sideInput = 0f;
            transform.RotateAround(playerPointer.transform.position, playerPointer.transform.up, Input.GetAxis("Horizontal"));
        }
        else
        {
            fwdInput = Input.GetAxis("Vertical");
            sideInput = Input.GetAxis("Horizontal");
        }
        Vector3 camForward2 = new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z) * fwdInput;
        Vector3 camRight2 = new Vector3(cam.transform.right.x, 0.0f, cam.transform.right.z) * sideInput;
        movement = camForward2 + camRight2;

        rb.AddForce(movement * speed);
    }

    void actionLogic()
    {
        // Cylinder movement input action
        if (mor == ShapeVar.CYLINDER)
        {
            if ((Input.GetAxis("Action") > 0) && grounded && charge < 50)
            {
                charge = charge + 1;

                // Increase slider value:
                chargeSlider.value += 2;                 cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);

                if (!csound.isPlaying)
                {
                    csound.Play();
                }
            }
            if ((Input.GetAxis("Action") > 0) && grounded && charge >= 50)
            {
                charge = charge + 1;

                // Increase slider value:
                if (chargeSlider.value < 99)                 {                     chargeSlider.value += 2;                     cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);
                    //Debug.Log("Charge slider value = " + chargeSlider.value);
                }

                if (!msound.isPlaying)
                {
                    msound.Play();
                }
            }
            if ((Input.GetAxis("Action") == 0) && grounded && charge >= 50)
            {
                msound.Stop();
                lsound.Play();
                Vector3 actionCl = (playerPointer.transform.forward * boost) + playerPointer.transform.up * 40;
                rb.AddForce(actionCl);
                charge = 0;
                // Reset slider value.
                chargeSlider.value = 0;                 cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);
            }
        }
        else if (mor == ShapeVar.CUBE)
        {
            Vector3 down = new Vector3(0.0f, -1f, 0.0f);
            if (shrunk)
            {
                if (grounded)
                {
                    shrink(false);
                }
                else
                {
                    if (Input.GetAxis("Action") > 0 && shrinkDelay <= 0)
                    {
                        shrink(false);
                        shrinkDelay = 0.4f;
                        down = down * glideFallSpeed;
                    }
                }
            }
            else
            {
                if (!grounded)
                {
                    if (Input.GetAxis("Action") > 0 && shrinkDelay <= 0)
                    {
                        shrink(true);
                        shrinkDelay = 0.4f;
                    }
                    else
                    {
                        down = down * heavyFallSpeed;
                    }
                }
            }
            rb.AddForce(down);
        }
        else
        {
            Vector3 camUpward = new Vector3(0.0f, 0.0f, 0.0f);
            if (grounded & (Input.GetAxis("Action") > 0))
            {
                camUpward = playerPointer.transform.up * (float)Input.GetAxis("Action") * ActionAmount;
                grounded = false;

               // if (!jsound.isPlaying)
               // {
               //     jsound.Play();
               // }
            }
            rb.AddForce(camUpward * speed);
        }
    }

    void shrink(bool b)
    {
        if (b)
        {
            if (!shrunk)
            {
                transform.localScale = transform.localScale + new Vector3(-0.7f, -0.7f, -0.7f);
                rb.useGravity = false;
                shrunk = true;
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            }
        }
        else
        {
            if (shrunk)
            {
                transform.localScale = transform.localScale + new Vector3(0.7f, 0.7f, 0.7f);
                rb.useGravity = true;
                shrunk = false;
            }
        }
    }

    void updateShape()
    {
        //Explosion effect for morphing
        var exp = GetComponent<ParticleSystem>();
        //currentCollisions = new List<GameObject>();

        ShapeVar next = ShapeVar.NONE;
        if (Input.GetKeyDown(KeyCode.Alpha1) && mor != ShapeVar.SPHERE)
            next = ShapeVar.SPHERE;
        if (Input.GetKeyDown(KeyCode.Alpha2) && mor != ShapeVar.CYLINDER)
            next = ShapeVar.CYLINDER;
        if (Input.GetKeyDown(KeyCode.Alpha3) && mor != ShapeVar.CUBE)
            next = ShapeVar.CUBE;

        if (next != ShapeVar.NONE)
        {
            exp.Play();
            if (mor == ShapeVar.SPHERE)
                GetComponent<SphereCollider>().enabled = false;
            else if (mor == ShapeVar.CYLINDER)
                GetComponent<MeshCollider>().enabled = false;
            else if (mor == ShapeVar.CUBE)
                GetComponent<BoxCollider>().enabled = false;

            if (next == ShapeVar.SPHERE)
            {
                GetComponent<SphereCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = sphere;
                chargeSlider.gameObject.SetActive(false);
                mor = ShapeVar.SPHERE;
            }
            else if (next == ShapeVar.CYLINDER)
            {
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cylinder;
                chargeSlider.gameObject.SetActive(true);
                mor = ShapeVar.CYLINDER;
            }
            else if (next == ShapeVar.CUBE)
            {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cube;
                chargeSlider.gameObject.SetActive(false);
                mor = ShapeVar.CUBE;
            }
        }
    }

    Vector3 getLowestVertex()
    {
        Vector3 lowest = new Vector3(0f, Mathf.Infinity, 0f);
        Mesh m = player.GetComponent<MeshFilter>().mesh;
        Vector3[] vecs = m.vertices;
        int inc = 12;
        if (mor == ShapeVar.CYLINDER)
            inc = 1;
        if (mor == ShapeVar.CUBE)
            inc = 3;
        if (vexCycler > inc)
            vexCycler = 0;
        for (int i = vexCycler; i < vecs.Length; i += inc)
        {
            //Debug.Log (i);
            Vector3 temp = player.transform.TransformVector(vecs[i]) + player.transform.position;

            if (temp.y < lowest.y)
                lowest = temp;

        }
        vexCycler++;
        return lowest;
    }

    void rayCastGround(Vector3 lowest)
    {
        int layerMask = LayerMask.GetMask("Default");
        RaycastHit hit;
        Ray ray = new Ray(lowest + new Vector3(0.0f, 0.01f, 0.0f), -playerPointer.transform.up);
        //Debug.Log (ray.origin.y);
        if (Physics.Raycast(ray, out hit, 0.09f, layerMask, QueryTriggerInteraction.Ignore))
        {
            grounded = true;
			if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("MovingGround"))
            {
                Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.yellow, 3f);
            }
			else
            {
                Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.red, 3f);
                grounded = false;
            }
        }
        else
        {
            grounded = false;
            Debug.DrawRay(ray.origin, ray.direction * 0.09f, Color.white, 3f);
        }
    }


    void LateUpdate()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        playerInBoundsCheck(other);
        checkpointCheck(other);
    }

    void playerInBoundsCheck(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            Debug.Log("Touching killbox");
            transform.position = spawn.transform.position;
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            // Update attempt number.
            attemptNo++;             SetAttemptText();
        }
    }

    void checkpointCheck(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            other.GetComponent<MeshRenderer>().enabled = false;
            this.spawn.transform.position = other.gameObject.transform.position;

            saveGame();
        }
    }

    void saveGame() {
        string curLevel = SceneManager.GetActiveScene().name;
        string checkpointNAme = spawn.name;

        string destination = Application.persistentDataPath + "/autosave.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(curLevel, checkpointNAme);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    void OnCollisionEnter(Collision col)
    {
        if (collidingWith != col.collider.gameObject && col.collider.gameObject.CompareTag("MovingGround"))
        {
            collidingWith = col.collider.gameObject;
            colliderTemp = new GameObject();
            colliderTemp.transform.parent = col.collider.transform;
            var scale = transform.localScale;
            transform.parent = colliderTemp.transform;
            transform.localScale = scale;
        }
        if (mor == ShapeVar.CUBE && col.collider.CompareTag("Breakable"))
        {
            Debug.Log("Blep");
            Vector3 momentum = rb.velocity * rb.mass; // p=mv
            if (momentum.magnitude >= breakingMomentum)
            {
                var exp = GetComponent<ParticleSystem>();
                Destroy(col.gameObject);
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.collider.gameObject.CompareTag("MovingGround"))
        {
            transform.parent = null;
            if (colliderTemp != null)
            {
                Destroy(colliderTemp);
                colliderTemp = null;
            }
            collidingWith = null;
        }
    }

    void SetAttemptText()     {         Debug.Log("Setting attempt text at attempt: " + attemptNo);         attemptText.text = "Attempt #" + attemptNo.ToString();         attemptText.enabled = true;         // Deactivate the text after 5 seconds.         StartCoroutine(deactivateText(5, attemptText));         Debug.Log("Called to deactivate text");     }

    /*      * Deactivates text after a set amount of time.      */
    IEnumerator deactivateText(int seconds, Text text)     {         Debug.Log("Deactivating text");         yield return new WaitForSeconds(seconds);         text.enabled = false;     }


}

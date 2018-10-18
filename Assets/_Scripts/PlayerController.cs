using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

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

    // Slider to visually show charging cylinder.
    public Slider chargeSlider;
    public Image cs_FillImage;                      
    public Color cs_FullChargeColor = Color.green;  
    public Color cs_ZeroChargeColor = Color.red; 

    // Displays the current attempt the player is on.
    public Text attemptText;
    private int attemptNo = 1;

    // Text box to display the time the player is taking for the level.
    public Text timerText;

    // Displays the text at the end of the level for when the player beats it.
    public Text finishText;
    public Text restartText;

    // The time the player took to complete the level.
    private float startTime;
    private float timeTaken;

    // True if the game is over.
    private bool gameWon = false;

    private int vexCycler = 0;

    List<GameObject> currentCollisions = new List<GameObject>();

    Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);

    // temp collider for collisions with moving platforms. needed for correct scaling
    private GameObject colliderTemp = null;
    // the object the player is colliding with to avoid collisions twice
    private GameObject collidingWith = null;

     void Awake()
    {
        if (StaticCheckpoint.spawn_point != null) {
            GameObject to_find = GameObject.Find(StaticCheckpoint.spawn_point);
            if (to_find != null) {
                spawn.transform.position = to_find.transform.position;
                Debug.Log("found a savegame!: checkpoint: " + StaticCheckpoint.spawn_point);
            }
        }    
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mor = ShapeVar.SPHERE;
        // Set the text for the current attempt.
        SetAttemptText();
        // Set the slider to be invisible at the start (so it only shows for the cylinder)
        chargeSlider.gameObject.SetActive(false);
        startTime = Time.time;
        gameWon = false;
        gameObject.transform.position = spawn.transform.position;
    }
    void Update()
    {
        if (!gameWon) {
            float deltaT = Time.deltaTime;
            shrinkDelay -= deltaT;
            //Debug.Log (1 / 4);
            rayCastGround(getLowestVertex());
            updateShape();

            movementLogic();



            updateTimer();

            //Debug.Log ("Grounded: " + grounded);
        }
        else
        {
            // Ask if the player wishes to restart.
            askRestart();
        }
    }

    private void FixedUpdate()
    {
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
                chargeSlider.value += 2;
                cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);

                if (!csound.isPlaying)
                {
                    csound.Play();
                }
            }
            if ((Input.GetAxis("Action") > 0) && grounded && charge >= 50)
            {
                charge = charge + 1;

                // Increase slider value:
                if (chargeSlider.value < 99)
                {
                    chargeSlider.value += 2;
                    cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);
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
                chargeSlider.value = 0;
                cs_FillImage.color = Color.Lerp(cs_ZeroChargeColor, cs_FullChargeColor, chargeSlider.value / 100);
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

                // The ball jumps at variable heights because it tries to counteract its current velocity. 
                // Sometimes it is adding to a negative down force (making it jump lower), and sometimes it is adding to
                // an up force, making it bounce extra high.

                // Resets the ball's upward velocity to be 0, so it can always achieve the same height on every jump.
                Vector3 vector3 = rb.velocity;
                vector3.y = 0;
                rb.velocity = vector3;

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
        if (shrunk && mor != ShapeVar.CUBE)
        {
            this.shrink(false);
        }
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
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                mor = ShapeVar.SPHERE;
            }
            else if (next == ShapeVar.CYLINDER)
            {
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cylinder;
                chargeSlider.gameObject.SetActive(true);
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                mor = ShapeVar.CYLINDER;
            }
            else if (next == ShapeVar.CUBE)
            {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cube;
                chargeSlider.gameObject.SetActive(false);
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
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

    void OnTriggerEnter(Collider other)
    {
        // Check if the player has won before checking the others in order to finish the game and ignore killboxes.
        playerWinCheck(other);
        playerInBoundsCheck(other);
        checkpointCheck(other);
    }

    void playerInBoundsCheck(Collider other)
    {
        if (other.CompareTag("KillBox") && !gameWon)
        {
            Debug.Log("Touching killbox");
            transform.position = spawn.transform.position;
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            // Update attempt number.
            attemptNo++;
            SetAttemptText();
        }
    }

    /*
     * CHECKPOINT EVENT HANDLER 
     * 
     */
    void checkpointCheck(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            other.GetComponent<MeshRenderer>().enabled = false;
            this.spawn.transform.position = other.gameObject.transform.position;
            this.spawn = other.gameObject;
            saveGame();
        }
    }

    void saveGame()
    {
        Debug.Log(Application.persistentDataPath);
        //save game parameters -> must correspond to attributes in the GameData C# class
        string curLevel = SceneManager.GetActiveScene().name;
        string checkpointName = this.spawn.name;


        List<GameData> savedGames = loadSavedGamesList();
        //get existing loadfiles to update them by overwrite a save, or adding a savefile;

        GameData to_save = new GameData(curLevel, checkpointName); // must correspond to attributes in the GameData C# class
        Debug.Log(to_save.checkPointName + "---" + to_save.levelName);
        if (savedGames.Count < 3)
        {
            savedGames.Add(to_save);
        }
        else
        {
            //overwriting via first come first serve
            for (int i = 0; i < 1; i++) {
                savedGames[i] = savedGames[i + 1];
            }
            savedGames[2] = to_save;
        }
        //transform to bypass queue object for serialization

        string saveDest = Application.persistentDataPath + "/autosave.dat";
        FileStream savefile;
        if (File.Exists(saveDest)) savefile = File.OpenWrite(saveDest);
        else savefile = File.Create(saveDest);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(savefile, savedGames);
        savefile.Close();

        Debug.Log("game has been saved  at: " + saveDest);
    }
    

    private List<GameData> loadSavedGamesList()
    {
        if (File.Exists(Application.persistentDataPath + "/autosave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadFile = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.OpenOrCreate);
            object tempQueue = bf.Deserialize(loadFile);
            Debug.Log("1 " + tempQueue.GetType().FullName);

            List<GameData> test = tempQueue as List<GameData>;
            Debug.Log("2 " + test);

            //QueueGamedata test2 = (QueueGamedata)tempQueue;
            //Debug.Log("3 " + test2);
            loadFile.Close();

            List<GameData> result = test;

            return result;
        }

        else {
            return new List<GameData>();
        }
    }

    void playerWinCheck(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            // Print win text and list the attempts and time taken.
            Debug.Log("Touching the finish");
            timeTaken = Time.time - startTime;
            displayFinishText();
            // End the game.
            gameWon = true;

        }
    }

    void askRestart() {
        restartText.text = "PRESS 'R' TO RESTART";
        if (Input.GetKeyDown("r")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
        if (mor == ShapeVar.SPHERE && col.collider.CompareTag("Water_Current"))
        {
            bob();
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

    // if in water, control the player's flotation etc
    void bob()
    {
        
    }

    void updateTimer(){
        // Code for updating the timer in the level.
        float currentTime = Time.time - startTime;
        int minutes = (int)currentTime / 60;
        int seconds = (int)currentTime % 60;
        if (seconds < 10)
        {
            timerText.text = "Time: " + minutes + ":0" + seconds;
        } else {
            timerText.text = "Time: " + minutes + ":" + seconds;
        }    
    }

    void SetAttemptText()
    {
        Debug.Log("Setting attempt text at attempt: " + attemptNo);
        attemptText.text = "Attempt #" + attemptNo.ToString();
        attemptText.enabled = true;
        // Deactivate the text after 5 seconds.
        StartCoroutine(deactivateText(5, attemptText));
        Debug.Log("Called to deactivate text");
    }

    void displayFinishText()
    {
        int minutes = (int) timeTaken / 60;
        int seconds = (int)timeTaken % 60;
        Debug.Log("Displaying finish text");
        if (seconds < 10){
            finishText.text = "Congratulations!\n Attempts taken: " + attemptNo + "\n Time taken: " + minutes + ":0" + seconds;
        } else {
            finishText.text = "Congratulations!\n Attempts taken: " + attemptNo + "\n Time taken: " + minutes + ":" + seconds;
        }
    }

    /*
     * Deactivates text after a set amount of time.
     */
    IEnumerator deactivateText(int seconds, Text text)
    {
        Debug.Log("Deactivating text");
        yield return new WaitForSeconds(seconds);
        text.enabled = false;
    }


}

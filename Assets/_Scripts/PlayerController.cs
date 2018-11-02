using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

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
    protected enum ShapeVar { SPHERE, CYLINDER, CUBE, TOP, NONE };

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
    public Mesh top;

    // Slider to visually show charging cylinder.
    public Slider chargeSlider;
    public Image cs_FillImage;                      
    public Color cs_FullChargeColor = Color.green;  
    public Color cs_ZeroChargeColor = Color.red; 

    // Animation for the spinning top     Animator m_animator;     bool spinning;     // Bool to check if the top has an item picked up:     bool haveItem;     public Transform pickUpLocation;     Transform pickedUpT = null;     GameObject pickedUpGO;

    // Displays the current attempt the player is on.
    public Text attemptText;
    private int attemptNo = 1;

    // Text box to display the time the player is taking for the level.
    public Text timerText;

    // Displays the text at the end of the level for when the player beats it.
    public Text finishText;
    public Text continueText;

    // The time the player took to complete the level.
    private float startTime;
    private float timeTaken;

    // True if the level has been beaten     private bool levelWon = false;     private bool gameWon = false;
     public LevelManager levelManager;

    private int vexCycler = 0;

    List<GameObject> currentCollisions = new List<GameObject>();

    Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
    private int fileNum;
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
                this.spawn = to_find.gameObject;
                Debug.Log("found a savegame!: checkpoint: " + StaticCheckpoint.spawn_point);
            }
        }    
    }
    void Start()
    {
        fileNum = PlayerPrefs.GetInt("SaveFile");
        rb = GetComponent<Rigidbody>();
        mor = ShapeVar.SPHERE;
        // Set the animator for the top and other related fields:
        m_animator = GetComponent<Animator>();         m_animator.enabled = false;         spinning = false;
        haveItem = false;
        // Set the text for the current attempt.
        SetAttemptText();
        // Set the slider to be invisible at the start (so it only shows for the cylinder)
        chargeSlider.gameObject.SetActive(false);
        startTime = Time.time;
        levelWon = false;
        gameObject.transform.position = spawn.transform.position;
    }
    void Update()
    {
        if (!levelWon) {
            float deltaT = Time.deltaTime;
            shrinkDelay -= deltaT;
            rayCastGround(getLowestVertex());
            updateShape();
            movementLogic();
            updateTimer();
        }
        else if (levelWon)
        {
            // Ask if the player wishes to restart.
            AskContinue();
        } else {
            WinGame();
        }
    }

    private void FixedUpdate()
    {
        if (!levelWon)
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
        else if (mor == ShapeVar.TOP)
        {             fwdInput = Input.GetAxisRaw("Vertical");             sideInput = Input.GetAxisRaw("Horizontal");         }
        else
        {
            fwdInput = Input.GetAxis("Vertical");
            sideInput = Input.GetAxis("Horizontal");
        }
        Vector3 camForward2 = new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z) * fwdInput;
        Vector3 camRight2 = new Vector3(cam.transform.right.x, 0.0f, cam.transform.right.z) * sideInput;
        movement = camForward2 + camRight2;

        if (!spinning)
            rb.AddForce(movement * speed);
        else {
            // Move without using the add force.
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            float dx = h * speed * Time.deltaTime * 0.05f;
            float dz = v * speed * Time.deltaTime * 0.05f;
            transform.position = new Vector3(transform.position.x + dx, transform.position.y, transform.position.z + dz);
        }
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

         
            }
            if ((Input.GetAxis("Action") == 0) && grounded && charge >= 50)
            {
                Vector3 launchForce = cam.transform.forward.normalized*40 + (cam.transform.up * 15);
                rb.AddForce(launchForce * boost);
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
        else if (mor == ShapeVar.TOP)         {             // Make the top spin by playing the animation.             if (Input.GetAxis("Action") > 0)             {                 // First play the animation to get it to stand back up. Then play the spinning animation straight after.                 m_animator.enabled = true;                 spinning = true;                 m_animator.speed = 1;                 //m_animator.Play("Start Up");                 m_animator.SetBool("Start_Spinning", true);                  //transform.position = transform.forward * Time.deltaTime * dz;                 //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                // Currently this controls how the top moves when standing still.
                // If set to FreezeAll, then the top stays still and only moves when input is provided.
                // However this makes it able to clip through walls if travelling at normal speed, which is unwanted.
                // Having constraints just set to FreezeRotation will ensure collisions happen, but the top moves on its own a bit with no input.                 rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;             } else {                 //GetComponent<Animator>().StopPlayback();                 // How to stop? Could set speed to 0.                 m_animator.SetBool("Start_Spinning", false);                 //m_animator.                 m_animator.enabled = false;                 spinning = false;                 if (haveItem)                 {                     pickedUpT.parent = null;                     Vector3 dropPos = new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);                     pickedUpT.transform.position = dropPos;                     pickedUpGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;                     pickedUpGO.GetComponent<Rigidbody>().useGravity = true;                     haveItem = false;                     pickedUpGO = null;                     pickedUpT = null;                 }                 rb.constraints = RigidbodyConstraints.None;                 //m_animator.speed = 0;                 //animation.Stop();             }          }
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
        if (Input.GetKeyDown(KeyCode.Alpha2) && mor != ShapeVar.CYLINDER && levelManager.GetCurrentLevel() >= 2)
            next = ShapeVar.CYLINDER;
        if (Input.GetKeyDown(KeyCode.Alpha3) && mor != ShapeVar.CUBE && levelManager.GetCurrentLevel() >= 3)
            next = ShapeVar.CUBE;
        if (Input.GetKeyDown(KeyCode.Alpha4) && mor != ShapeVar.TOP && levelManager.GetCurrentLevel() >= 4)             next = ShapeVar.TOP; 
        if (next != ShapeVar.NONE)
        {
            exp.Play();
            if (mor == ShapeVar.SPHERE)
                GetComponent<SphereCollider>().enabled = false;
            else if (mor == ShapeVar.CYLINDER)
                GetComponent<MeshCollider>().enabled = false;
            else if (mor == ShapeVar.CUBE)
                GetComponent<BoxCollider>().enabled = false;
            else if (mor == ShapeVar.TOP)
                GetComponent<MeshCollider>().enabled = false;
            
            if (next == ShapeVar.SPHERE)
            {
                GetComponent<SphereCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = sphere;
                chargeSlider.gameObject.SetActive(false);
                spinning = false;
                speed = 80;                 m_animator.enabled = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                mor = ShapeVar.SPHERE;
            }
            else if (next == ShapeVar.CYLINDER)
            {
                GetComponent<MeshCollider>().sharedMesh = cylinder;
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cylinder;
                chargeSlider.gameObject.SetActive(true);
                spinning = false;                 m_animator.enabled = false;
                speed = 240;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                mor = ShapeVar.CYLINDER;
            }
            else if (next == ShapeVar.CUBE)
            {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<MeshFilter>().mesh = cube;
                chargeSlider.gameObject.SetActive(false);
                spinning = false;
                speed = 80;                 m_animator.enabled = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                mor = ShapeVar.CUBE;
            }
            else if (next == ShapeVar.TOP)             {                 // Need to change the mesh collider mesh to be top instead of cylinder.                 GetComponent<MeshCollider>().sharedMesh = top;                 GetComponent<MeshCollider>().enabled = true;                 GetComponent<MeshFilter>().mesh = top;                 chargeSlider.gameObject.SetActive(false);
                speed = 80;                 rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;                 mor = ShapeVar.TOP;             } 
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
			if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("MovingGround") || hit.collider.CompareTag("Water_Current"))
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
        if (other.CompareTag("KillBox") && !levelWon)
        {
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
    /*
     * Checks if collided with an object marked pickupable with the tag 'Pickup'.
     * 
     */
    void pickUpCheck(Collider other){         if (other.CompareTag("Pickup") && spinning && !haveItem){             haveItem = true;             //other.transform.SetParent(this.transform);             pickedUpGO = other.gameObject;             pickedUpGO.GetComponent<Rigidbody>().useGravity = false;             pickedUpT = other.transform;             // How to get this to be floating above this.transform rather than on the side of it             // or wherever contact is made?             pickedUpT.position = pickUpLocation.position;             pickedUpT.transform.SetParent(transform);             pickedUpGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;         }     } 
    int findWithAttr(List<GameData> checkpoints, string checkpoint)
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (checkpoints[i].checkPointName.Equals(checkpoint))
            {
                return i;
            }
        }
        return -1;
    }
    List<GameData> getCorrectLevel(List<GameData> data) {
        //only access savepoint in same scene
        List<GameData> temp = new List<GameData>();
        foreach (GameData gameData in data) {
            if (gameData.levelName == SceneManager.GetActiveScene().name) {
                temp.Add(gameData);
            }
        }
        return temp;
    }
    void saveGame()
    {
        //save game parameters -> must correspond to attributes in the GameData C# class
        string curLevel = SceneManager.GetActiveScene().name;
        string checkpointName = this.spawn.name;
        //get existing loadfiles to update them by overwrite a save, or adding a savefile;
        List<GameData> savedGames = loadSavedGamesList();
       //checks if checkpoint already exists
        
        //Creates no duplicates
        int savedGameIndex = findWithAttr(savedGames, checkpointName);
        if (savedGameIndex != -1)
        {
            return;
        }
        //removes all saves from last level
        savedGames = getCorrectLevel(savedGames);
        GameData to_save = new GameData(curLevel, checkpointName); // must correspond to attributes in the GameData C# class
        
        if (savedGames.Count < 3)
        {
            savedGames.Add(to_save);
        }
        else
        {
            //overwriting via first come first serve
            for (int i = 0; i < 2; i++) {
                savedGames[i] = savedGames[i + 1];
            }
            savedGames[2] = to_save;
        }
        //transform to bypass queue object for serialization

        string saveDest = Application.persistentDataPath + "/autosave"+fileNum+".dat";
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
        string fileName = "autosave" + fileNum + ".dat";
        if (File.Exists(Application.persistentDataPath +"/"+fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();
            //QueueGamedata test2 = (QueueGamedata)tempQueue;
            //Debug.Log("3 " + test2);
            //file.Close();

            List<GameData> result = savedGames;

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
            // End the level if not on the last level.
            if (!levelManager.OnLastLevel())
            {
                levelWon = true;
            } else {
                gameWon = true;
            }

        }
    }

    private void AskContinue()
    {         continueText.text = "PRESS 'R' TO RESTART OR ENTER TO CONTINUE";         if (Input.GetKeyDown("r"))         {             levelManager.RestartLevel();         }         else if (Input.GetKeyDown(KeyCode.Return))         {
            // Takes the user to the next level.
            StaticCheckpoint.spawn_point = "";
            levelManager.LoadNextLevel();         }      }

    private void WinGame()
    {         continueText.text = "PRESS 'R' TO RESTART";         if (Input.GetKeyDown("r"))
        {             levelManager.RestartLevel();         }     }


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
        if (mor == ShapeVar.SPHERE && (col.collider.CompareTag("Water_Current") || col.collider.CompareTag("Water_Still")));
        {
            bob();
        }
        if (mor == ShapeVar.CUBE && !shrunk && col.collider.CompareTag("Breakable"))
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
        pickUpCheck(col.collider);
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
    private void bob()
    {
        // rb.AddForce(new Vector3(0.0f, 1.0f, 0.0f));
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
        attemptText.text = "Attempt #" + attemptNo.ToString();
        attemptText.enabled = true;
        // Deactivate the text after 5 seconds.
        StartCoroutine(deactivateText(5, attemptText));
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
        //Debug.Log("Deactivating text");
        yield return new WaitForSeconds(seconds);
        text.enabled = false;
    }


}

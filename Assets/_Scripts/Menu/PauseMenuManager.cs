using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseMenuManager : MonoBehaviour {
   public List<Button> buttons = new List<Button>();
   private bool isPaused = false;
    public GameObject pauseMenu;
	// Use this for initialization
	void Start () {
        this.pauseMenu.SetActive(false);

    }
   
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            this.isPaused = !this.isPaused;
        
        if (isPaused)
        {
            this.pauseMenu.SetActive(true);
                Time.timeScale = 0;
        }
        else {
            this.pauseMenu.SetActive(false);
               Time.timeScale = 1;
            }
        }
    }
}

using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseMenuManager : MenuManager {
   public List<Button> pauseButtons = new List<Button>();
    private List<Action> pauseActions;
    private bool isPaused = false;
    public GameObject pauseMenu;
    private Action escape;
	// Use this for initialization
	void Start () {
        this.pauseMenu.SetActive(false);
        this.initActions();
        this.initButtons();
        base.setActions(this.pauseActions);
        base.setButtons(this.pauseButtons);
        base.setEscapeAction(this.escape);
        base.setMenuObject(this.pauseMenu);
    }
    void initButtons()
    {
        this.pauseButtons[0].onClick.AddListener(Resume);
        this.pauseButtons[1].onClick.AddListener(Resume);
        this.pauseButtons[2].onClick.AddListener(Resume);
        this.pauseButtons[3].onClick.AddListener(Quit);
    }
    void initActions()
    {
        this.pauseActions = new List<Action>();
        this.initEscapeAction();
        this.pauseActions.Add(() => Resume());
        this.pauseActions.Add(() => Resume());
        this.pauseActions.Add(() => Resume());
        this.pauseActions.Add(() => Quit());
    }

    void initEscapeAction() {
        this.escape = (() => onPressedEscape());

    }
  
    public void onPressedEscape() {
        this.isPaused = !this.isPaused;
        if (isPaused)
        {
            this.pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            this.Resume();
        }
    }
    public void Resume() {
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit() {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }
}

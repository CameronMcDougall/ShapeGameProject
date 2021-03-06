﻿using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StartMenuManager : MenuManager {
    /* 
    * Start Menu for Shape game; MDDN243/COMP313 course Project
    * Cameron McDougall; mcdougcame@myvuw.ac.nz
    */
    public List<Button> startButtons;
    private List<Action> startActions;
    private Action escape;
    void Start()
    {
        base.setMenuObject(this.gameObject);
        this.initActions();
        this.initButtons();
        base.setActions(this.startActions);
        base.setButtons(this.startButtons);
        base.setEscapeAction(escape);
    }
    void initButtons() {
        //initialise mouse presses
        this.startButtons[0].onClick.AddListener(onStart);
        this.startButtons[1].onClick.AddListener(onLoad);
        this.startButtons[2].onClick.AddListener(onQuit);
    }
    void initActions() {
        //Creates all the button actions
        this.startActions = new List<Action>();
        this.initEscape();
        this.startActions.Add(() => onStart());
        this.startActions.Add(() => onLoad());
        this.startActions.Add(() => onQuit());
    }

    void initEscape() {
        this.escape = () => onQuit();
    }
    int findLastFile() {
        //gets last file number
        DirectoryInfo d = new DirectoryInfo(@Application.persistentDataPath);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.dat"); //Getting Text files
        return Files.Length+1;
    }
    void onStart()
    {
        //init the file number 
        PlayerPrefs.SetInt("SaveFile", findLastFile());
        SceneManager.LoadScene("Level_1");
        StaticCheckpoint.spawn_point = "";
    }
    void onLoad()
    {
        SceneManager.LoadScene("LoadMenu");
    }

    void onQuit()
    {
        Application.Quit();
    }

}

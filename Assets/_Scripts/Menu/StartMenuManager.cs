using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StartMenuManager : MenuManager {
    public List<Button> buttons;
    private List<Action> actions;
    private Action escape;
    void Start()
    {
        this.initActions();
        this.initButtons();
        base.setActions(this.actions);
        base.setButtons(this.buttons);
        base.setEscapeAction(escape);
        base.setMenuObject(this.gameObject);
    }
    void initButtons() {
        this.buttons[0].onClick.AddListener(onStart);
        this.buttons[1].onClick.AddListener(onLoad);
        this.buttons[2].onClick.AddListener(onQuit);
    }
    void initActions() {
        this.actions = new List<Action>();
        this.initEscape();
        actions.Add(() => onStart());
        actions.Add(() => onLoad());
        actions.Add(() => onQuit());
    }

    void initEscape() {
        this.escape = () => onQuit();
    }
    int findLastFile() {
        DirectoryInfo d = new DirectoryInfo(@Application.persistentDataPath);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.dat"); //Getting Text files
        return Files.Length+1;
    }
    void onStart()
    {
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

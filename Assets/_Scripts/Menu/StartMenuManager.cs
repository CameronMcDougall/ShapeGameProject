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
        this.buttons[2].onClick.AddListener(onSelectLevel);
        this.buttons[3].onClick.AddListener(onQuit);
    }
    void initActions() {
        this.actions = new List<Action>();
        this.initEscape();
        actions.Add(() => onStart());
        actions.Add(() => onLoad());
        actions.Add(() => onSelectLevel());
        actions.Add(() => onQuit());
    }

    void initEscape() {
        this.escape = () => onQuit();
    }
    
    void onStart()
    {
        SceneManager.LoadScene("Level_1");
    }
    void onLoad()
    {
        /// here is to loading the game
        if (File.Exists(Application.persistentDataPath + "/autosave.dat")) {

            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>) bf.Deserialize(file);
            file.Close();

            //Queue < GameData > savedGames = tempQueue.savesQueue;
            GameData to_load = savedGames[savedGames.Count-1];
            StaticCheckpoint.spawn_point = to_load.checkPointName;
            SceneManager.LoadScene(to_load.levelName);
        }
    }

    void onSelectLevel()
    {

    }
    void onQuit()
    {
        Application.Quit();
    }

}

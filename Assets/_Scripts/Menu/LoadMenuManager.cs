using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LoadMenuManager : MenuManager {

    public List<Button> buttons;
    private List<Action> actions;
    private List<GameData> savedGames;
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
    void initButtons()
    {
        this.buttons[0].onClick.AddListener(loadSaveGame1);
        this.buttons[1].onClick.AddListener(loadSaveGame2);
        this.buttons[2].onClick.AddListener(loadSaveGame3);
        this.buttons[3].onClick.AddListener(onBack);

        this.getSaveFiles();

        if (savedGames != null) {
            for (int i = 0; i < savedGames.Count; i++) {
                GameData data = savedGames[savedGames.Count - (1 + i)];
                this.buttons[i].GetComponentInChildren<Text>().text =
                    data.levelName + " | " + data.checkPointName; 
            }
        }
    }

    void getSaveFiles()
    {
        if (File.Exists(Application.persistentDataPath + "/autosave.dat"))
        {
            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.OpenOrCreate);
            savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();
        }
    }

    void initActions()
    {
        this.actions = new List<Action>();
        this.initEscape();
        actions.Add(() => loadSaveGame1());
        actions.Add(() => loadSaveGame2());
        actions.Add(() => loadSaveGame3());
        actions.Add(() => onBack());
    }

    void initEscape()
    {
        this.escape = () => onBack();
    }

    void loadSaveGame1() {

    }
    void loadSaveGame2() {
    }
    void loadSaveGame3() {
    }


    void onBack ()
    {
        Application.Quit();
    }

    // Use this for initialization

    // Update is called once per frame
    void Update () {
		
	}
}

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
        this.buttons[0].onClick.AddListener(delegate { loadSaveGame(0);});
        this.buttons[1].onClick.AddListener(delegate { loadSaveGame(1);});
        this.buttons[2].onClick.AddListener(delegate { loadSaveGame(2);});
        this.buttons[3].onClick.AddListener(onBack);

        //gets and print save files to buttons
        
        this.getSaveFiles();
        if (savedGames != null) {
            for (int i = 0; i < savedGames.Count; i++) {
                GameData data = savedGames[i];
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
            Debug.Log(Application.persistentDataPath);
        }
    }

    void initActions()
    {
        this.actions = new List<Action>();
        this.initEscape();
        actions.Add(() => loadSaveGame(0));
        actions.Add(() => loadSaveGame(1));
        actions.Add(() => loadSaveGame(2));
        actions.Add(() => onBack());
    }

    void initEscape()
    {
        this.escape = () => onBack();
    }

    void loadSaveGame(int num)
    {
        if (File.Exists(Application.persistentDataPath + "/autosave.dat"))
        {
            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();

            //Queue < GameData > savedGames = tempQueue.savesQueue;
            if (num <= savedGames.Count - 1) {
                GameData to_load = savedGames[num];
                StaticCheckpoint.spawn_point = to_load.checkPointName;
                SceneManager.LoadScene(to_load.levelName);
            }
        }
    }


    void onBack ()
    {
        SceneManager.LoadScene("StartMenu");
    }

    // Use this for initialization

    // Update is called once per frame
    void Update () {
		
	}
}

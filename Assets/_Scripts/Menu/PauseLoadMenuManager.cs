using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PauseLoadMenuManager : MenuManager {

    public List<Button> loadButtons;
    private List<Action> loadActions;
    private List<GameData> savedGames;
    private Action escape;
    void Start()
    {
        this.initActions();
        this.initButtons();
        base.setActions(this.loadActions);
        base.setButtons(this.loadButtons);
        base.setEscapeAction(escape);
        base.setMenuObject(this.gameObject);
    }
    void initButtons()
    {
        this.loadButtons[0].onClick.AddListener(delegate { loadSaveGame(0); });
        this.loadButtons[1].onClick.AddListener(delegate { loadSaveGame(1); });
        this.loadButtons[2].onClick.AddListener(delegate { loadSaveGame(2); });
        this.loadButtons[3].onClick.AddListener(onBack);

        //gets and print save files to buttons

        this.getSaveFiles();
        if (savedGames != null)
        {
            print(savedGames.Count);
            for (int i = 0; i < savedGames.Count; i++)
            {
                GameData data = savedGames[i];
                this.loadButtons[i].GetComponentInChildren<Text>().text =
                    data.levelName + " | " + data.checkPointName;
            }
        }

    }

    void getSaveFiles()
    {
        if (File.Exists(Application.persistentDataPath + "/autosave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadFile = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.OpenOrCreate);
            object tempQueue = bf.Deserialize(loadFile);
            List<GameData> test = tempQueue as List<GameData>;
            loadFile.Close();
            this.savedGames = test;
            print(savedGames.Count);
        }


    }

    void initActions()
    {
        this.loadActions = new List<Action>();
        this.initEscape();
        this.loadActions.Add(() => loadSaveGame(0));
        this.loadActions.Add(() => loadSaveGame(1));
        this.loadActions.Add(() => loadSaveGame(2));
        this.loadActions.Add(() => onBack());
    }

    void initEscape()
    {
        this.escape = () => onBack();
    }
    private int smallestAvaiableSave()
    {
        DirectoryInfo d = new DirectoryInfo(@Application.persistentDataPath);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
        string str = "";
        foreach (FileInfo file in Files)
        {
            str = str + ", " + file.Name;
        }

        return 0;
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
            if (num <= savedGames.Count - 1)
            {
                GameData to_load = savedGames[num];
                StaticCheckpoint.spawn_point = to_load.checkPointName;
                SceneManager.LoadScene(to_load.levelName);
            }
        }
    }


    void onBack()
    {
        SceneManager.LoadScene("StartMenu");
    }


}

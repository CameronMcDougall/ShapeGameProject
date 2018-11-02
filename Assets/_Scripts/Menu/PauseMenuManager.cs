using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class PauseMenuManager : MenuManager
{
    public List<Button> pauseButtons = new List<Button>();
    private List<Action> pauseActions;
    private bool isPaused = false;
    public GameObject pauseMenu;
    //  public GameObject loadMenu;
    private int fileNum;
    private Action escape;
    // Use this for initialization
    void Start()
    {
        base.setMenuObject(this.pauseMenu);
        this.pauseMenu.SetActive(false);
        this.initActions();
        this.initButtons();
        base.setActions(this.pauseActions);
        base.setButtons(this.pauseButtons);
        base.setEscapeAction(this.escape);
       
        this.fileNum = PlayerPrefs.GetInt("SaveFile");
    }
    void initButtons()
    {
        this.pauseButtons[0].onClick.AddListener(Resume);
        this.pauseButtons[1].onClick.AddListener(onLoad);
        this.pauseButtons[2].onClick.AddListener(Quit);
    }
    void initActions()
    {
        this.pauseActions = new List<Action>();
        this.initEscapeAction();
        this.pauseActions.Add(() => Resume());
        this.pauseActions.Add(() => onLoad());
       // this.pauseActions.Add(() => Resume());
        this.pauseActions.Add(() => Quit());
    }

    void initEscapeAction()
    {

        this.escape = (() => onPressedEscape());

    }

    public void onPressedEscape()
    {
      // if (!this.loadMenu.activeInHierarchy)
        // {
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
        this.resetIndex();
    //    }
    }
    public void Resume()
    {
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    void loadSaveGame()
    {
        print(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/autosave" + fileNum + ".dat"))
        {
            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave" + fileNum + ".dat", FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();

            //Queue < GameData > savedGames = tempQueue.savesQueue;

            this.resetIndex();
            this.pauseMenu.SetActive(false);
            
            Time.timeScale = 1;
            GameData to_load = savedGames[savedGames.Count - 1];
            foreach (GameData data in savedGames) {
                print(data.checkPointName);
            }
            if (SceneManager.GetActiveScene().name != to_load.levelName)
                return;
            StaticCheckpoint.spawn_point = to_load.checkPointName;
            SceneManager.LoadScene(to_load.levelName);
        }
    }
    public void onLoad()
    {
        this.resetIndex();
        loadSaveGame();
    }
    public void Quit()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }
}

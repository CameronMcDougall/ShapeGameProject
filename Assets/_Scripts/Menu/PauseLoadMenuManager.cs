using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PauseLoadMenuManager : MenuManager {

    private List<Button> loadButtons;
    private List<Action> loadActions;
    private List<GameData> savedGames;
    private Action escape;
    private int fileNum;
    public GameObject buttonPref;
    public GameObject checkpointMenu;
    public GameObject pauseMenu;
    private float time;
    void Start()
    {
        base.setMenuObject(this.checkpointMenu);
        this.checkpointMenu.SetActive(false);
        this.fileNum = PlayerPrefs.GetInt("SaveFile");
        this.initButtons();
        base.setActions(this.loadActions);
        base.setButtons(this.loadButtons);
        base.setEscapeAction(escape);
        this.enabled = false;
    }
    void initButtons()
    {
        this.loadButtons = new List<Button>();
        this.loadActions = new List<Action>();
        //gets and print save files to buttons
        this.getSaveFiles();
        float yOffset = 40;
        if (savedGames != null)
        { 
            Transform transform = this.transform;
            transform.position += new Vector3(0, 40, 0);
            for (int i = 0; i < savedGames.Count; i++)
            {
                GameData data = savedGames[i];
                print(data.checkPointName);
                this.createButton(data.levelName + " | " + data.checkPointName, transform);
                this.loadButtons[i].GetComponent<RectTransform>().position += new Vector3(0, yOffset, 0);
                yOffset -= loadButtons[i].GetComponent<RectTransform>().rect.height + 5;
                this.loadActions.Add(() => onLoadAction(i));
            }  
        }
        this.createButton("Back", transform);
        this.loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().position += new Vector3(0, yOffset, 0);
        this.loadButtons[this.loadButtons.Count - 1].GetComponentInChildren<Text>().fontSize = 14;
        this.loadActions.Add(() => onBack());
        this.initEscape();
    }
    private void OnEnable()
    {
        print("enabled");
        time = Time.unscaledTime;

    }
    void onLoadAction(int num) {
       loadSaveGame(num); 
    }
    void getSaveFiles()
    {
        if (File.Exists(Application.persistentDataPath + "/autosave" +this.fileNum+ ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadFile = File.Open(Application.persistentDataPath + "/autosave" + this.fileNum + ".dat", FileMode.OpenOrCreate);
            object tempQueue = bf.Deserialize(loadFile);
            List<GameData> test = tempQueue as List<GameData>;
            loadFile.Close();
            this.savedGames = test;
        }


    }
    void createButton(string text, Transform pos)
    {
        GameObject button = Instantiate(buttonPref, pos);
        button.transform.parent = this.checkpointMenu.transform;
        Button but = button.GetComponent<Button>();
        but.GetComponentInChildren<Text>().text = text;
        loadButtons.Add(but);
    }

    void initEscape()
    {
        this.escape = () => onBack();
    }
   
    void loadSaveGame(int num)
    {
        if (File.Exists(Application.persistentDataPath + "/autosave"+fileNum+".dat"))
        {
            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave"+fileNum+".dat", FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();

            //Queue < GameData > savedGames = tempQueue.savesQueue;
            if (num <= savedGames.Count - 1)
            {
                this.resetIndex();
                this.checkpointMenu.SetActive(false);
                Time.timeScale = 1;
                GameData to_load = savedGames[num];
                StaticCheckpoint.spawn_point = to_load.checkPointName;
                SceneManager.LoadScene(to_load.levelName);
                
            }
        }
    }

    void onBack()
    {
        if (this.checkpointMenu.activeInHierarchy) {
            this.checkpointMenu.SetActive(false);
            GameObject pauseObject = GameObject.Find("Menus");
          
            this.pauseMenu.SetActive(true);
            pauseObject.GetComponent<PauseMenuManager>().enabled = true;
            this.resetIndex();
            this.GetComponent<PauseLoadMenuManager>().enabled = false;
        }  
    }


}

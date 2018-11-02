using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LoadMenuManager : MenuManager
{
    /* 
     * Load Menu for Shape game; MDDN243/COMP313 course Project
     * Cameron McDougall; mcdougcame@myvuw.ac.nz
     * 
     * Modified by Quan Lee; lequan1@myvuw.ac.nz
    */
    public GameObject buttonPref;
    private List<Button> loadButtons;
    private List<Action> loadActions;
    private List<GameData> savedGames;
    private Action escape;
    void Start()
    {
        this.loadActions = new List<Action>();
        this.loadButtons = new List<Button>();
        this.getSaves();
        base.setActions(this.loadActions);
        base.setButtons(this.loadButtons);
        base.setEscapeAction(escape);
        base.setMenuObject(this.gameObject);
    }

    void createButton(string text, string date, Transform pos)
    {
        //creates load button with date and text
        GameObject button = Instantiate(buttonPref, pos);
        Button but = button.GetComponent<Button>();
        but.GetComponentInChildren<Text>().text = text + "- " + date;
        loadButtons.Add(but);
    }

    void initEscape()
    {
        this.escape = () => onBack();
    }
    private void getSaves()
    {
        DirectoryInfo d = new DirectoryInfo(@Application.persistentDataPath);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.dat"); //Getting Text files
        Transform transform = this.transform;
        float yOffset = 0;
        for (int index = Files.Length - 1; index >= 0; index--)
        {
            //gets file from latest creation
            FileInfo file = Files[index];
            List<GameData> savedGames = getLoadData(file.Name);
            GameData to_load = savedGames[savedGames.Count - 1];
            //creates button using time checkpoint past and level name
            this.createButton(to_load.levelName.Replace("_", " "), to_load.timestamp, transform);
            //moves button down
            this.loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().position += new Vector3(0, yOffset, 0);
            this.loadActions.Add(() => loadSaveGame(file.Name, index));
            this.loadButtons[this.loadButtons.Count - 1].onClick.AddListener(delegate { loadSaveGame(file.Name, index);});
            yOffset -= loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().rect.height + 5;
            //make text spread over button (no cut offs)
            this.loadButtons[this.loadButtons.Count - 1].GetComponentInChildren<Text>().fontSize = 12;
        }
        //back button and escape action creation
        this.createButton("Back", "", transform);
        this.loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().position += new Vector3(0, yOffset, 0);
        this.loadButtons[this.loadButtons.Count - 1].GetComponentInChildren<Text>().fontSize = 14;
        this.loadActions.Add(() => onBack());
        this.loadButtons[this.loadButtons.Count - 1].onClick.AddListener(onBack);
        this.initEscape();
    }
    List<GameData> getLoadData(string fileName)
    {
        //getting the list of saved games and closing the fileOpener
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
        List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
        file.Close();
        return savedGames;
    }
    void loadSaveGame(string fileName, int index)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            List<GameData> savedGames = getLoadData(fileName);
            //Queue < GameData > savedGames = tempQueue.savesQueue;
            string temp = fileName;
            temp = temp.Replace("autosave", "");
            temp = temp.Replace(".dat", "");
            int saveFileNum = (temp != "") ? Int32.Parse(temp) : 1;
            PlayerPrefs.SetInt("SaveFile", saveFileNum);
            //Gets furthest checkpoint
            GameData to_load = savedGames[savedGames.Count - 1];
            StaticCheckpoint.spawn_point = to_load.checkPointName;
            SceneManager.LoadScene(to_load.levelName);
        }
    }
    void onBack()
    {
        SceneManager.LoadScene("StartMenu");
    }


}

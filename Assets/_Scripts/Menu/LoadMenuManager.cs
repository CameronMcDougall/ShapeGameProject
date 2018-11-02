using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LoadMenuManager : MenuManager
{
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

    void createButton(string text, string date,Transform pos)
    { 
        GameObject button = Instantiate(buttonPref,pos);
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
        for (int i = Files.Length-1; i >= 0; i--)
        {
            FileInfo file = Files[i];
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + "/" + file.Name, FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(f);
            f.Close();

            //Queue < GameData > savedGames = tempQueue.savesQueue;

            GameData to_load = savedGames[0];
            this.createButton(to_load.levelName, to_load.timestamp, transform);
            this.loadButtons[this.loadButtons.Count -1 ].GetComponent<RectTransform>().position += new Vector3(0,yOffset,0);
            this.loadActions.Add(()=> loadSaveGame(file.Name));
            yOffset -= loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().rect.height + 5;
        }
        print(transform.position);
        this.createButton("Back", "", transform);
        this.loadButtons[this.loadButtons.Count - 1].GetComponent<RectTransform>().position += new Vector3(0, yOffset, 0);
        this.loadButtons[this.loadButtons.Count - 1].GetComponentInChildren<Text>().fontSize = 14;
        this.loadActions.Add(() => onBack());
        this.initEscape();
    }

    void loadSaveGame(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            //getting the list of saved games and closing the fileOpener
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
            List<GameData> savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();
            //Queue < GameData > savedGames = tempQueue.savesQueue;
            string temp = fileName;
            temp = temp.Replace("autosave","");
            temp = temp.Replace(".dat", "");
            int saveFileNum = (temp != "") ? Int32.Parse(temp): 1;
            PlayerPrefs.SetInt("SaveFile", saveFileNum);
            GameData to_load = savedGames[savedGames.Count-1];
            StaticCheckpoint.spawn_point = to_load.checkPointName;
            SceneManager.LoadScene(to_load.levelName);

        }
    }


    void onBack()
    {
        SceneManager.LoadScene("StartMenu");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    /*
     * Manages the scenes for the Shape Game:
     * Lorens (Leo) Hansen; hansenlore@myvuw.ac.nz
     * 30/10/18
     */

    public static LevelManager Instance { set; get;}

    private int currentLevel = -1;

    private int lastLevel = 3;

    void Awake()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadScene(string name) {
        Debug.Log("Level load requested for: " + name);
        SceneManager.LoadScene(name);
    }

    public void LoadNextLevel(){
        if (currentLevel <= 2)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        // else end the game
    }

    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public string GetCurrentLevelName(){
        return SceneManager.GetActiveScene().name;
    }

    public int GetCurrentLevel() {
        return currentLevel;
    }

    public bool OnLastLevel() {
        return currentLevel == lastLevel;
    }

}

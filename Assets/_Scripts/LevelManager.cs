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

    void Awake()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex - 3;
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

    public int getCurrentLevel() {
        return currentLevel;
    }

}

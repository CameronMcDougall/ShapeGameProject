using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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

    }

    void onSelectLevel()
    {

    }
    void onQuit()
    {
        Application.Quit();
    }

}

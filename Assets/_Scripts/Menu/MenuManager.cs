using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour { 
    protected List<Button> buttons;
    private int amountOfButtons, currentSelection;
    protected List<Action> actions;
    protected Action onEscape;
    private bool textToUpdate, hasMoved,hasEscaped;
    private float maxTimeToMove = 1, timeMoved;
    private float timeEscaped;
    public GameObject menu;
    protected virtual void setButtons(List<Button> buttons) {
        this.buttons = buttons;
        amountOfButtons = buttons.Count;
        buttons[0].image.color = Color.green;
    }
    protected virtual void setActions(List<Action> actions)
    {
        this.actions = actions;
    }
    protected virtual void setMenuObject(GameObject obj)
    {
        this.menu = obj;
    }
    protected virtual void setEscapeAction(Action action)
    {
        this.onEscape = action;
    }
    void onEnter() {
        if (currentSelection < 0 || currentSelection >= amountOfButtons)
            return;
        actions[currentSelection].Invoke();
    }
    void updateText() {
        for (int i = 0; i < amountOfButtons; ++i) {
            if (i == currentSelection) {
                buttons[i].image.color = Color.green;
            }
            else {
                buttons[i].image.color = Color.white;
            }  
        }
    }
    private void Update()
    {
       print("current selection: "+this.currentSelection);
        if (hasEscaped)
        {
            if (Time.unscaledTime - this.timeEscaped >= this.maxTimeToMove)
            {
                this.hasEscaped = false;
            }
        }
        // print("Has Escaped:" + this.hasEscaped);
        if (!this.hasEscaped)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                this.onEscape.Invoke();
                this.timeEscaped = Time.unscaledTime;
                this.hasEscaped = true;
            }
        }
        if (menu.activeInHierarchy)
        {
            
            if (Input.GetKey(KeyCode.W))
            {

                if (!hasMoved)
                {
                    currentSelection = (--currentSelection < 0) ? 0 : currentSelection;
                    textToUpdate = true;
                    hasMoved = true;
                    this.timeMoved = Time.unscaledTime;
                }

            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (!hasMoved)
                {
                    this.currentSelection = (++this.currentSelection >= amountOfButtons) ? amountOfButtons - 1 : currentSelection;
                    textToUpdate = true;
                    hasMoved = true;
                    this.timeMoved = Time.unscaledTime;
                }
            }
            else if (Input.GetKey(KeyCode.Return))
            {
                onEnter();
                textToUpdate = false;
            }
            else
            {
                textToUpdate = false;
            }
            if (textToUpdate)
            {
                updateText();
            }
            if (hasMoved)
            {
                if (Time.unscaledTime - this.timeMoved >= this.maxTimeToMove)
                {
                    this.hasMoved = false;
                }
            }
           
        }
    }
}

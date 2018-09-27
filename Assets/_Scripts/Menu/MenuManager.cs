using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour { 
    protected List<Button> buttons;
    private int amountOfButtons, currentSelection;
    protected List<Action> actions;
    private bool textToUpdate, hasMoved;
    private float maxTimeToMove =1, timeMoved;
    protected virtual void setButtons(List<Button> buttons) {
        this.buttons = buttons;
        amountOfButtons = buttons.Count;
        buttons[0].image.color = Color.green;
    }
    protected virtual void setActions(List<Action> actions)
    {
        this.actions = actions;
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
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (!hasMoved)
            {
                currentSelection = (--currentSelection < 0) ? 0 : currentSelection;
                textToUpdate = true;
                hasMoved = true;
                this.timeMoved = Time.time;
            }
            
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (!hasMoved)
            {
                this.currentSelection = (++this.currentSelection >= amountOfButtons) ? amountOfButtons - 1 : currentSelection;
                textToUpdate = true;
                hasMoved = true;
                this.timeMoved = Time.time;
            }
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            onEnter();
            textToUpdate = false;
        }
        else {
            textToUpdate = false;
        }
        if (textToUpdate)
        {
            updateText();
        }
        if (hasMoved) {
            if (Time.time - this.timeMoved >= this.maxTimeToMove) {
                this.hasMoved = false;
            }
        }
    }
}

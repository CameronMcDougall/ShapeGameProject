using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    /* 
    * Menu Manager component for Shape game; MDDN243/COMP313 course Project
    * Cameron McDougall; mcdougcame@myvuw.ac.nz
    */
    protected List<Button> buttons;
    private int amountOfButtons, currentSelection;
    protected List<Action> actions;
    protected Action onEscape;
    private bool textToUpdate, hasMoved, hasEscaped;
    private float maxTimeToMove = 1, timeMoved;
    private float timeEscaped;
    public GameObject menu;
    protected virtual void setButtons(List<Button> buttons)
    {
        this.buttons = buttons;
        this.amountOfButtons = buttons.Count;
        this.buttons[0].image.color = Color.green;
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
    void onEnter()
    {
        if (currentSelection < 0 || currentSelection >= amountOfButtons)
            return;
        this.actions[this.currentSelection].Invoke();
    }
    void updateText()
    {
        //sets the current selection to green toggle
        for (int i = 0; i < amountOfButtons; ++i)
        {
            if (i == this.currentSelection)
            {
                this.buttons[i].image.color = Color.green;
            }
            else
            {
                this.buttons[i].image.color = Color.white;
            }
        }
    }
    public void resetIndex()
    {
        this.currentSelection = 0;
    }
    void handleUserInput()
    {
        //Handles key input for the menu movement and actions
        if (Input.GetKey(KeyCode.W))
        {
            //Menu moved up
            if (!hasMoved)
            {
                currentSelection = (--currentSelection < 0) ? 0 : currentSelection;
                this.cursorMoved();
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //Menu moved down
            if (!this.hasMoved)
            {
                this.currentSelection = (++this.currentSelection >= amountOfButtons) ? amountOfButtons - 1 : currentSelection;
                this.cursorMoved();
            }
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            this.onEnter();
            this.textToUpdate = false;
        }
        else
        {
            this.textToUpdate = false;
        }
    }
    void cursorMoved()
    {
        textToUpdate = true;
        hasMoved = true;
        this.timeMoved = Time.unscaledTime;
    }
    private void Update()
    {

        if (hasEscaped)
        {
            //stops updating every frame
            if (Time.unscaledTime - this.timeEscaped >= this.maxTimeToMove)
            {
                this.hasEscaped = false;
            }
        }
        else
        {
            //On Escape
            if (Input.GetKey(KeyCode.Escape))
            {
                this.onEscape.Invoke();
                this.timeEscaped = Time.unscaledTime;
                this.hasEscaped = true;
            }
        }
        //if not disabled
        if (menu.activeInHierarchy)
        {
            this.handleUserInput();
            if (this.textToUpdate)
            {
                this.updateText();
            }
            if (this.hasMoved)
            {
                if (Time.unscaledTime - this.timeMoved >= this.maxTimeToMove)
                {
                    this.hasMoved = false;
                }
            }

        }
    }
}

using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseMenuManager : MonoBehaviour {
    List<Button> buttons = new List<Button>();
    Canvas canvas;
	// Use this for initialization
	void Start () {
        this.canvas = GetComponent<Canvas>();

	}
    public void CreateButton(Transform canvas, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        button.transform.parent = canvas;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta = size;
        button.GetComponent<Button>().onClick.AddListener(method);
    }
    // Update is called once per frame
    void Update () {
		
	}
}

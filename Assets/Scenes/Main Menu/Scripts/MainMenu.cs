﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    void Start()
    {
        Gondola.cerealTargets = new List<GameObject>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame(){
        SceneManager.LoadSceneAsync(1);
    }

    public void Credits() {
        SceneManager.LoadSceneAsync(3);
    }

    public void QuitGame() {
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoSettings : MonoBehaviour {

    private void Start(){
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }
}

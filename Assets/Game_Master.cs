using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Manages all the overhead 
public class Game_Master : MonoBehaviour
{
    // Start is called before the first frame update
    public Character_Movement_3D controller;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GameObject.Find("Player").GetComponent<Character_Movement_3D>();

        Gondola.cerealTargets[Random.Range(0, Gondola.cerealTargets.Count)].GetComponent<Gondola>().mustSpawnCereal = true;
        Debug.Log(Gondola.cerealTargets.Count);
        
        foreach (GameObject g in Gondola.cerealTargets)
        {
            g.GetComponent<Gondola>().PlaceShelfItemsV2();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.m_health <= 0){
            Game_Over();
        }

    }

    void Game_Over(){
        SceneManager.LoadScene("Main Menu");
    }
}

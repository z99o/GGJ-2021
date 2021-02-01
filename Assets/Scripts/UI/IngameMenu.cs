using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    
    public GameObject ingameMenu;

    public GameObject resumeGameBtn;
    public GameObject showControlsBtn;
    public GameObject returnMainBtn;
    public GameObject exitBtn;


    public GameObject controlsMenu;

    public GameObject hideControlsBtn;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        resumeGameBtn.GetComponent<Button>().onClick.AddListener(() => { ResumeGame(); });
        showControlsBtn.GetComponent<Button>().onClick.AddListener(() => { ShowControls(); });
        hideControlsBtn.GetComponent<Button>().onClick.AddListener(() => { HideControls(); });

        returnMainBtn.GetComponent<Button>().onClick.AddListener(() => { Time.timeScale = 1f; SceneManager.LoadSceneAsync("Main Menu"); });
        exitBtn.GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if(!isPaused)
            {
                PauseGame();
            } else
            {
                ResumeGame();

            }
        }

    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        ingameMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        ingameMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;

    }

    public void ShowControls()
    {
        controlsMenu.SetActive(true);
        ingameMenu.SetActive(false);
    }

    public void HideControls()
    {
        controlsMenu.SetActive(false);
        ingameMenu.SetActive(true);
    }


   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Canvas pauseUI;
    private PlayerMovement playerMovement;
    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseUI = GameObject.Find("PauseUI").GetComponent<Canvas>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pauseUI.enabled = false;
    }

    // ESCAPE is pressed    
    public void Pause(InputAction.CallbackContext context)
    {
        paused = true;
        pauseUI.enabled = true;
        playerMovement.MoveInput = Vector2.zero;
        Time.timeScale = 0;
    }

    // CONTINUE is pressed (button 1)
    public void Continue()
    {
        paused = false;
        pauseUI.enabled = false;
        Time.timeScale = 1f;
    }
   
    // SETTINGS is pressed (button 2)
    public void Settings(int scene)
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(scene);
    }

    // QUIT is pressed (button 3)
    public void Quit(int scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
}

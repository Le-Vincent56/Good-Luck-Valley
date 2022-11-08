using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!paused)
        {
            paused = true;
            pauseUI.enabled = true;
            Time.timeScale = 0;
        } else
        {
            paused = false;
            pauseUI.enabled = false;
            Time.timeScale = 1;
        }
    }
}

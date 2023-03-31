using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region REFERENCES
    private Canvas pauseUI;
    private PlayerMovement playerMovement;
    private Journal journalMenu;
    private Button pauseSettingsButton;
    #endregion

    #region FIELDS
    [SerializeField] private bool paused = false;
    #endregion

    #region PROPERTIES
    public bool Paused { get { return paused; } set { paused = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalMenu = GameObject.Find("JournalUI").GetComponent<Journal>();
        pauseUI = GameObject.Find("PauseUI").GetComponent<Canvas>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pauseSettingsButton = GameObject.Find("Settings Button").GetComponent<Button>();
        pauseUI.enabled = false;
    }

    public void Update()
    {
        pauseSettingsButton.targetGraphic.color = pauseSettingsButton.colors.disabledColor;
        pauseSettingsButton.interactable = false;
    }

    /// <summary>
    /// Pause the Game
    /// </summary>
    /// <param name="context">The context of the Controller</param>    
    public void Pause(InputAction.CallbackContext context)
    {
        paused = true;
        pauseUI.enabled = true;
        playerMovement.MoveInput = Vector2.zero;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Continue the Game
    /// </summary>
    public void Continue()
    {
        if(!journalMenu.MenuOpen)
        {
            paused = false;
            pauseUI.enabled = false;
            Time.timeScale = 1f;
        }
    }
   
    /// <summary>
    /// Take the Player to Settings screen
    /// </summary>
    /// <param name="scene">The scene number that represents the Settings scene</param>
    public void Settings(int scene)
    {
        if(!journalMenu.MenuOpen)
        {
            Time.timeScale = 0f;
            SceneManager.LoadScene(scene);
        }
    }

    /// <summary>
    /// Quit to Title
    /// </summary>
    /// <param name="scene">Scene number that represents Quitting to Title</param>
    public void Quit(int scene)
    {
        if(!journalMenu.MenuOpen)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
        }
    }
}

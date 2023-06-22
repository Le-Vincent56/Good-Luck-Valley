using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour, IData
{
    #region REFERENCES
    private Canvas pauseUI;
    private PlayerMovement playerMovement;
    private Journal journalMenu;
    [SerializeField] private SaveSlotsPauseMenu saveMenu;
    #endregion

    #region FIELDS
    [SerializeField] private bool paused = false;
    private string levelName;
    private float playtimeTotal;
    private float playtimeHours;
    private float playtimeMinutes;
    private float playtimeSeconds;
    private string playtimeString;
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
        saveMenu = GameObject.Find("SaveUI").GetComponent<SaveSlotsPauseMenu>();
        pauseUI.enabled = false;

        levelName = SceneManager.GetActiveScene().name;

        // Start time record coroutine
        StartCoroutine(RecordTimeRoutine());
    }

    /// <summary>
    /// Toggle the pause menu
    /// </summary>
    /// <param name="context">The context of the controller</param>
    public void TogglePause(InputAction.CallbackContext context)
    {
        if((!journalMenu.MenuOpen && journalMenu.CloseBuffer <= 0) && (!saveMenu.MenuOpen && saveMenu.CloseBuffer <= 0))
        {
            if (!paused)
            {
                paused = true;
                pauseUI.enabled = true;
                playerMovement.MoveInput = Vector2.zero;
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                pauseUI.enabled = false;
                Time.timeScale = 1f;
            }
        }
    }

    /// <summary>
    /// Continue the Game
    /// </summary>
    public void Continue()
    {
        if(!journalMenu.MenuOpen && !saveMenu.MenuOpen)
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
        if(!journalMenu.MenuOpen && !saveMenu.MenuOpen)
        {
            Time.timeScale = 0f;
            SceneManager.LoadScene(scene);
        }
    }

    /// <summary>
    /// Save the Game
    /// </summary>
    public void Save()
    {
        // Disable pause UI
        pauseUI.enabled = false;

        // Activate the save menu
        saveMenu.ActivateMenu();
    }

    /// <summary>
    /// Quit to Title
    /// </summary>
    /// <param name="scene">Scene number that represents Quitting to Title</param>
    public void Quit(int scene)
    {
        if(!journalMenu.MenuOpen && !saveMenu.MenuOpen)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
        }
    }

    /// <summary>
    /// Coroutine to record playtime
    /// </summary>
    /// <returns>The total time played stored in variables</returns>
    public IEnumerator RecordTimeRoutine()
    {
        TimeSpan ts;
        while(!paused)
        {
            // Record playtime every second
            yield return new WaitForSeconds(1);
            playtimeTotal += 1;

            // Turn playtime into hours, minutes, and seconds
            ts = TimeSpan.FromSeconds(playtimeTotal);
            playtimeHours = (int)ts.TotalHours;
            playtimeMinutes = ts.Minutes;
            playtimeSeconds = ts.Seconds;

            // Create a playtime string
            playtimeString = playtimeHours + ":" + playtimeMinutes + ":" + playtimeSeconds;
        }
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        levelName = data.levelName;
        playtimeString = data.playtime;
    }

    public void SaveData(GameData data)
    {
        data.levelName = levelName;
        data.playtime = playtimeString;
    }
    #endregion
}

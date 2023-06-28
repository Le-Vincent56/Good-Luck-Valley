using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] DisableScriptableObj disableEvent;
    private Canvas pauseUI;
    private Canvas settingsUI;
    private PlayerMovement playerMovement;
    private Journal journalMenu;
    [SerializeField] private SaveSlotsPauseMenu saveMenu;
    #endregion

    #region FIELDS
    [SerializeField] private bool paused = false;
    private bool canPause = true;
    private string levelName;
    private float playtimeTotal;
    private float playtimeHours;
    private float playtimeMinutes;
    private float playtimeSeconds;
    private string playtimeString;
    #endregion

    #region PROPERTIES
    public bool Paused { get { return paused; } set { paused = value; } }
    public bool CanPause { get { return canPause; } set {  canPause = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalMenu = GameObject.Find("JournalUI").GetComponent<Journal>();
        pauseUI = GameObject.Find("PauseUI").GetComponent<Canvas>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        saveMenu = GameObject.Find("SaveUI").GetComponent<SaveSlotsPauseMenu>();
        pauseUI.enabled = false;
        settingsUI = GameObject.Find("SettingsUI").GetComponent<Canvas>();  
        settingsUI.enabled = false;

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
        if((!journalMenu.MenuOpen && journalMenu.CloseBuffer <= 0) && (!saveMenu.MenuOpen && saveMenu.CloseBuffer <= 0) && canPause && !settingsUI.enabled)
        {
            if (!paused)
            {
                paused = true;
                pauseUI.enabled = true;
                Time.timeScale = 0;
                disableEvent.Pause();
            }
            else
            {
                paused = false;
                pauseUI.enabled = false;
                Time.timeScale = 1f;
                disableEvent.Unpause();
            }
        }
    }

    /// <summary>
    /// Continue the Game
    /// </summary>
    public void Continue()
    {
        if(!journalMenu.MenuOpen && !saveMenu.MenuOpen && !settingsUI.enabled)
        {
            paused = false;
            pauseUI.enabled = false;
            Time.timeScale = 1f;
            disableEvent.Unpause();
        }
    }
   
    /// <summary>
    /// Take the Player to Settings screen
    /// </summary>
    /// <param name="scene">The scene number that represents the Settings scene</param>
    public void Settings()
    {
        if(!journalMenu.MenuOpen && !saveMenu.MenuOpen && !settingsUI.enabled)
        {
            paused = true;
            pauseUI.enabled = false;
            Time.timeScale = 0f;
            settingsUI.enabled = true;
        }
    }

    public void CloseSettings()
    {
        Debug.Log("Close Settings");
        pauseUI.enabled = true;
        settingsUI.enabled = false;
        paused = true;
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
            if (AudioManager.Instance)
            {
                AudioManager.Instance.AmbienceEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                AudioManager.Instance.MusicEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
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
        while(!paused)
        {
            // Record playtime every second
            yield return new WaitForSeconds(1);
            playtimeTotal += 1;
        }
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        levelName = data.levelName;
        playtimeTotal = data.playtimeTotal;
        playtimeString = data.playtimeString;
    }

    public void SaveData(GameData data)
    {
        #region CALCULATE PLAYTIME
        // Turn playtime into hours, minutes, and seconds
        playtimeHours = (playtimeTotal / 3600) % 24;
        playtimeMinutes = (playtimeTotal / 60) % 60;
        playtimeSeconds = playtimeTotal % 60;

        // Create a playtime string
        playtimeString = playtimeHours + ":" + playtimeMinutes + ":" + playtimeSeconds;
        #endregion

        data.levelName = levelName;
        data.playtimeTotal = playtimeTotal;
        data.playtimeString = playtimeString;
    }
    #endregion
}

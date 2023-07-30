using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PauseScriptableObject", menuName = "ScriptableObjects/Pause Event")]
public class PauseScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool canPause = true;
    [SerializeField] private bool paused = false;
    [SerializeField] private bool pauseMenuOpen = false;

    #region EVENTS
    public UnityEvent pauseEvent;
    public UnityEvent unpauseEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (pauseEvent == null)
        {
            pauseEvent = new UnityEvent();
        }

        if (unpauseEvent == null)
        {
            unpauseEvent = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set whether the player can pause or not
    /// </summary>
    /// <param name="canPause">Whether the player can pause or not</param>
    public void SetCanPause(bool canPause)
    {
        this.canPause = canPause;
    }

    /// <summary>
    /// Set whether the game is paused or not
    /// </summary>
    /// <param name="paused">Whether the game is paused or not</param>
    public void SetPaused(bool paused)
    {
        this.paused = paused;
    }

    /// <summary>
    /// SEt whether the pause menu is open or not
    /// </summary>
    /// <param name="pauseMenuOpen">Whether the pause menu is open or not</param>
    public void SetPauseMenuOpen(bool pauseMenuOpen)
    {
        this.pauseMenuOpen = pauseMenuOpen;
    }

    /// <summary>
    /// Get whether the player can pause or not
    /// </summary>
    /// <returns>Whether the player can pause or not</returns>
    public bool GetCanPause()
    {
        return canPause;
    }

    /// <summary>
    /// Get whether the game is paused or not
    /// </summary>
    /// <returns>Whether the game is paused or not</returns>
    public bool GetPaused()
    {
        return paused;
    }

    /// <summary>
    /// Get whether the pause menu is open or not
    /// </summary>
    /// <returns>Whether the pause menu is open or not</returns>
    public bool GetPauseMenuOpen()
    {
        return pauseMenuOpen;
    }

    /// <summary>
    /// Trigger events related to pausing the game
    /// </summary>
    public void Pause()
    {
        if(canPause)
        {
            paused = true;
            pauseEvent.Invoke();
        }
    }

    /// <summary>
    /// Trigger events relaed to unpausing the game
    /// </summary>
    public void Unpause()
    {
        paused = false;
        unpauseEvent.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        canPause = true;
        paused = false;
        pauseMenuOpen = false;
    }
}

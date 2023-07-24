using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DisableScriptableObject", menuName = "ScriptableObjects/Disable Event")]
public class DisableScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool playerLocked;
    [SerializeField] private bool inputEnabled;
    [SerializeField] private bool disableParallax;
    [SerializeField] private float inputCooldown;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent lockPlayerEvent;
    public UnityEvent unlockPlayerEvent;
    public UnityEvent<float> stopInputEvent;
    public UnityEvent disablePlayerInputEvent;
    public UnityEvent enablePlayerInputEvent;
    public UnityEvent enableHUD;
    public UnityEvent disableHUD;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (lockPlayerEvent == null)
        {
            lockPlayerEvent = new UnityEvent();
        }

        if (unlockPlayerEvent == null)
        {
            unlockPlayerEvent = new UnityEvent();
        }

        if (stopInputEvent == null)
        {
            stopInputEvent = new UnityEvent<float>();
        }

        if (disablePlayerInputEvent == null)
        {
            disablePlayerInputEvent = new UnityEvent();
        }

        if (enablePlayerInputEvent == null)
        {
            enablePlayerInputEvent = new UnityEvent();
        }

        if(enableHUD == null)
        {
            enableHUD = new UnityEvent();
        }

        if (disableHUD == null)
        {
            disableHUD = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set input cooldown
    /// </summary>
    /// <param name="inputCooldown">The length of the input cooldown</param>
    public void SetInputCooldown(float inputCooldown)
    {
        this.inputCooldown = inputCooldown;
    }

    /// <summary>
    /// Get whether the player is locked or not
    /// </summary>
    /// <returns>Whether the player is locked or not</returns>
    public bool GetPlayerLocked()
    {
        return playerLocked;
    }

    /// <summary>
    /// Get whether the player has input enabled or not
    /// </summary>
    /// <returns>Whether the player has input enabled or not</returns>
    public bool GetInputEnabled()
    {
        return inputEnabled;
    }

    /// <summary>
    /// Trigger events related to locking the player
    /// </summary>
    public void Lock()
    {
        playerLocked = true;
        lockPlayerEvent.Invoke();
    }

    /// <summary>
    /// Trigger events related to unlocking the player
    /// </summary>
    public void Unlock()
    {
        playerLocked = false;
        unlockPlayerEvent.Invoke();
    }

    /// <summary>
    /// Trigger events related to stopping input on a cooldown
    /// </summary>
    public void StopInput()
    {
        stopInputEvent.Invoke(inputCooldown);
    }

    /// <summary>
    /// Trigger events relating to disabling input
    /// </summary>
    public void DisableInput()
    {
        inputEnabled = false;
        disablePlayerInputEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to enabling input
    /// </summary>  
    public void EnableInput()
    {
        inputEnabled = true;
        enablePlayerInputEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to enabling the HUD
    /// </summary>
    public void EnableHUD()
    {
        enableHUD.Invoke();
    }

    /// <summary>
    /// Trigger events relating to disabling the HUD
    /// </summary>
    public void DisableHUD()
    {
        disableHUD.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        playerLocked = false;
        inputEnabled = true;
        inputCooldown = 0f;
    }

    public void SetDisableParallax(bool disableParallax)
    {
        this.disableParallax = disableParallax;
    }

    public bool GetDisableParallax()
    {
        return disableParallax;
    }
}

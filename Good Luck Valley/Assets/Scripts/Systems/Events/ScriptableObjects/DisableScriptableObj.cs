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
    [SerializeField] private float inputCooldown;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent lockPlayerEvent;
    public UnityEvent unlockPlayerEvent;
    public UnityEvent<float> stopInputEvent;
    public UnityEvent disablePlayerInputEvent;
    public UnityEvent enablePlayerInputEvent;
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
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        playerLocked = false;
        inputEnabled = true;
        inputCooldown = 0f;
    }
}

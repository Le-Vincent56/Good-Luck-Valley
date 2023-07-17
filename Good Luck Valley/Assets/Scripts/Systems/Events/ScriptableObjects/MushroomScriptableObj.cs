using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MushroomScriptableObject", menuName = "ScriptableObjects/Mushroom Event")]
public class MushroomScriptableObj : ScriptableObject, IData
{
    #region FIELDS
    [SerializeField] private bool touchingShroom;
    [SerializeField] private bool throwUnlocked;
    [SerializeField] private bool throwing;
    [SerializeField] Vector3 bounceForce;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent<Vector3, ForceMode2D> bounceEvent;
    public UnityEvent<bool> touchingShroomEvent;
    public UnityEvent unlockThrowEvent;
    public UnityEvent checkThrowAnimationEvent;
    public UnityEvent setThrowAnimationEvent;
    #endregion
    #endregion

    #region PROPERTIES
    public bool IsTouchingShroom { get {  return touchingShroom; } } 
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        

        if (touchingShroomEvent == null)
        {
            touchingShroomEvent = new UnityEvent<bool>();
        }

        if(checkThrowAnimationEvent == null)
        {
            checkThrowAnimationEvent = new UnityEvent();
        }

        if (setThrowAnimationEvent == null)
        {
            setThrowAnimationEvent = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set whether the player is touching a shroom
    /// </summary>
    /// <param name="touchingShroom">Whether the player is touching a shroom</param>
    public void SetTouchingShroom(bool touchingShroom)
    {
        this.touchingShroom = touchingShroom;
    }

    /// <summary>
    /// Set whether the player has unlocked the shroom throw or not
    /// </summary>
    /// <param name="throwUnlocked">Whether the player has unlocked the shroom throw or not</param>
    public void SetThrowUnlocked(bool throwUnlocked)
    {
        this.throwUnlocked = throwUnlocked;
    }

    /// <summary>
    /// Set whether the player is throwing
    /// </summary>
    /// <param name="throwing">Whether the player is throwing</param>
    public void SetThrowing(bool throwing)
    {
        this.throwing = throwing;
    }

    public bool GetThrowing()
    {
        return throwing;
    }

    /// <summary>
    /// Get whether the player has gotten the throw ability or not
    /// </summary>
    /// <returns>Whether the player has gotten the throw ability or not</returns>
    public bool GetThrowUnlocked()
    {
        return throwUnlocked;
    }

    /// <summary>
    /// Trigger events related to touching a shroom
    /// </summary>
    public void TouchingShroom()
    {
        touchingShroomEvent.Invoke(touchingShroom);
    }

    /// <summary>
    /// Trigger events related to getting the shroom throw ability
    /// </summary>
    public void UnlockThrow()
    {
        throwUnlocked = true;
        unlockThrowEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to checking throwing animations
    /// </summary>
    public void CheckThrow()
    {
        checkThrowAnimationEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to setting a throwing animatino
    /// </summary>
    public void SetThrowAnim()
    {
        setThrowAnimationEvent.Invoke();
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        throwing = false;
    }

    public void SaveData(GameData data)
    {

    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MushroomScriptableObject", menuName = "ScriptableObjects/Mushroom Event")]
public class MushroomScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool bouncing;
    [SerializeField] private bool touchingShroom;
    [SerializeField] private bool throwing;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent bounceEvent;
    public UnityEvent<bool> bounceAnimationEvent;
    public UnityEvent<bool> touchingShroomEvent;
    public UnityEvent checkThrowAnimationEvent;
    public UnityEvent<bool> setThrowAnimationEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (bounceEvent == null)
        {
            bounceEvent = new UnityEvent();
        }

        if (bounceAnimationEvent == null)
        {
            bounceAnimationEvent = new UnityEvent<bool>();
        }

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
            setThrowAnimationEvent = new UnityEvent<bool>();
        }
        #endregion
    }

    /// <summary>
    /// Set whether the player is bouncing
    /// </summary>
    /// <param name="bouncing">Whether the player is bouncing</param>
    public void SetBounce(bool bouncing)
    {
        this.bouncing = bouncing;
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
    /// Set whether the player is throwing
    /// </summary>
    /// <param name="throwing">Whether the player is throwing</param>
    public void SetThrowing(bool throwing)
    {
        this.throwing = throwing;
    }

    /// <summary>
    /// Trigger bounce-related events
    /// </summary>
    public void Bounce()
    {
        bounceEvent.Invoke();
        bounceAnimationEvent.Invoke(bouncing);
    }

    /// <summary>
    /// Trigger events related to touching a shroom
    /// </summary>
    public void TouchingShroom()
    {
        touchingShroomEvent.Invoke(touchingShroom);
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
        setThrowAnimationEvent.Invoke(throwing);
    }
}

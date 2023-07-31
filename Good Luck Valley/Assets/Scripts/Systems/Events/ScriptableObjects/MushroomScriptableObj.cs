using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MushroomScriptableObject", menuName = "ScriptableObjects/Mushroom Event")]
public class MushroomScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool touchingShroom;
    [SerializeField] private bool throwUnlocked;
    [SerializeField] private bool throwing;
    [SerializeField] private bool firstFull;
    [SerializeField] private bool firstThrow;
    [SerializeField] private bool firstBounce;
    [SerializeField] private bool showingMaxMessage;
    [SerializeField] private bool touchgingFastFall;
    [SerializeField] Vector3 bounceForce;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent<Vector3, ForceMode2D> bounceEvent;
    public UnityEvent<bool> touchingShroomEvent;
    public UnityEvent unlockThrowEvent;
    public UnityEvent checkThrowAnimationEvent;
    public UnityEvent setThrowAnimationEvent;
    public UnityEvent endThrowEvent;
    public UnityEvent clearShroomsEvent;
    public UnityEvent showThrowMessageEvent;
    public UnityEvent hideThrowMessageEvent;
    public UnityEvent showBounceMessageEvent;
    public UnityEvent hideBounceMessageEvent;
    public UnityEvent showMaxMessageEvent;
    public UnityEvent hideMaxMessageEvent;
    public UnityEvent showFastFallMessageEvent;
    public UnityEvent hideFastFallMessageEvent;
    public UnityEvent showWallBounceMessageEvent;
    public UnityEvent hideWallBounceMessageEvent;
    #endregion
    #endregion

    #region PROPERTIES
    public bool IsTouchingShroom { get { return touchingShroom; } }
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (touchingShroomEvent == null)
        {
            touchingShroomEvent = new UnityEvent<bool>();
        }

        if (checkThrowAnimationEvent == null)
        {
            checkThrowAnimationEvent = new UnityEvent();
        }

        if (setThrowAnimationEvent == null)
        {
            setThrowAnimationEvent = new UnityEvent();
        }

        if (endThrowEvent == null)
        {
            endThrowEvent = new UnityEvent();
        }

        if (clearShroomsEvent == null)
        {
            clearShroomsEvent = new UnityEvent();
        }

        if (showThrowMessageEvent == null)
        {
            showThrowMessageEvent = new UnityEvent();
        }

        if (hideThrowMessageEvent == null)
        {
            hideThrowMessageEvent = new UnityEvent();
        }

        if (showBounceMessageEvent == null)
        {
            showBounceMessageEvent = new UnityEvent();
        }

        if (hideBounceMessageEvent == null)
        {
            hideBounceMessageEvent = new UnityEvent();
        }

        if (showMaxMessageEvent == null)
        {
            showMaxMessageEvent = new UnityEvent();
        }

        if (hideMaxMessageEvent == null)
        {
            hideMaxMessageEvent = new UnityEvent();
        }

        if (showFastFallMessageEvent == null)
        {
            showFastFallMessageEvent = new UnityEvent();
        }

        if (hideFastFallMessageEvent == null)
        {
            hideFastFallMessageEvent = new UnityEvent();
        }

        if (showWallBounceMessageEvent == null)
        {
            showWallBounceMessageEvent = new UnityEvent();
        }

        if (hideWallBounceMessageEvent == null)
        {
            hideWallBounceMessageEvent = new UnityEvent();
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

    /// <summary>
    /// Set the bounce force of the last mushroom bounced on
    /// </summary>
    /// <param name="bounceForce">Bounce force of the last mushroom bounced on</param>
    public void SetBounceForce(Vector3 bounceForce)
    {
        this.bounceForce = bounceForce;
    }

    /// <summary>
    /// Set whether the player has not yet thrown for the first time or not
    /// </summary>
    /// <param name="firstThrow">Whether the player has not yet thrown for the first time or not</param>
    public void SetFirstThrow(bool firstThrow)
    {
        this.firstThrow = firstThrow;
    }

    /// <summary>
    /// Set whether the player has not yet bounced for the first time or not
    /// </summary>
    /// <param name="firstBounce">whether the player has not yet bounced for the first time or not</param>
    public void SetFirstBounce(bool firstBounce)
    {
        this.firstBounce = firstBounce;
    }

    /// <summary>
    /// Set whether the player has hit the maximum mushroom limit for the first time or not
    /// </summary>
    /// <param name="firstFull">Whether the player has hit the maximum mushroom limit for the first time or not</param>
    public void SetFirstFull(bool firstFull)
    {
        this.firstFull = firstFull;
    }

    /// <summary>
    /// Get whether the player is throwing or not
    /// </summary>
    /// <returns></returns>
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
    /// Get whether it is the player's first time throwing a mushroom or not
    /// </summary>
    /// <returns>Whether it is the player's first time throwing a mushroom or not</returns>
    public bool GetFirstThrow()
    {
        return firstThrow;
    }

    /// <summary>
    /// Get whether it is the player's first time bouncing on a mushroom or not
    /// </summary>
    /// <returns>Whether it is the player's first time bouncing on a mushroom or not</returns>
    public bool GetFirstBounce()
    {
        return firstBounce;
    }

    /// <summary>
    /// Get whether it is the player's first time hitting max mushrooms or not
    /// </summary>
    /// <returns>Whether it is the player's first time hitting max mushrooms or not</returns>
    public bool GetFirstFull()
    {
        return firstFull;
    }

    /// <summary>
    /// Get whether the max mushroom tutorial message is showing or not
    /// </summary>
    /// <returns>Whether the max mushroom tutorial message is showing or not</returns>
    public bool GetShowingMaxMessage()
    {
        return showingMaxMessage;
    }

    public Vector3 GetBounceForce()
    {
        return bounceForce;
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

    /// <summary>
    /// Trigger events relating to ending the throwing state
    /// </summary>
    public void EndThrow()
    {
        endThrowEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to clearing shrooms
    /// </summary>
    public void ClearShrooms()
    {
        clearShroomsEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to showing the mushroom throw message
    /// </summary>
    public void ShowThrowMessage()
    {
        showThrowMessageEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to hiding the mushroom throw message
    /// </summary>
    public void HideThrowMessage()
    {
        firstThrow = false;
        hideThrowMessageEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to showing the mushroom bounce message
    /// </summary>
    public void ShowBounceMessage()
    {
        showBounceMessageEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to hiding the mushroom bounce message
    /// </summary>
    public void HideBounceMessage()
    {
        firstBounce = false;
        hideBounceMessageEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to showing the mushroom max message
    /// </summary>
    public void ShowMaxMessage()
    {
        firstFull = true;
        showingMaxMessage = true;
        showMaxMessageEvent.Invoke();
    }

    /// <summary>
    /// Trigger events relating to hiding the mushroom max message
    /// </summary>
    public void HideMaxMessage()
    {
        showingMaxMessage = false;
        hideMaxMessageEvent.Invoke();
    }

    public void ShowFastFallMessage()
    {
        showFastFallMessageEvent.Invoke();
    }

    public void HideFastFallMessage()
    {
        showFastFall = false;
        hideFastFallMessageEvent.Invoke();
    }

    public void ShowWallBounceMessage()
    {
        showWallBounceMessageEvent.Invoke();
    }

    public void HideWallBounceMessage()
    {
        hideWallBounceMessageEvent.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        touchingShroom = false;
        throwUnlocked = false;
        throwing = false;
        showingMaxMessage = false;
        bounceForce = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PauseScriptableObject", menuName = "ScriptableObjects/Pause Event")]
public class PauseScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool canPause = true;

    #region EVENTS
    public UnityEvent pauseEvent;
    public UnityEvent unpauseEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        if (pauseEvent == null)
        {
            pauseEvent = new UnityEvent();
        }

        if (unpauseEvent == null)
        {
            unpauseEvent = new UnityEvent();
        }
    }

    public void SetCanPause(bool canPause)
    {
        this.canPause = canPause;
    }

    public bool GetCanPause()
    {
        return canPause;
    }

    /// <summary>
    /// Trigger events related to pausing the game
    /// </summary>
    public void Pause()
    {
        if(canPause)
        {
            pauseEvent.Invoke();
        }
    }

    /// <summary>
    /// Trigger events relaed to unpausing the game
    /// </summary>
    public void Unpause()
    {
        unpauseEvent.Invoke();
    }
}

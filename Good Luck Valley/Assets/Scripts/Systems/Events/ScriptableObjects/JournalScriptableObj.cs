using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "JournalScriptableObject", menuName = "ScriptableObjects/Journal Event")]
public class JournalScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool hasJournal = false;
    [SerializeField] private bool journalOpen = false;
    [SerializeField] private bool openedOnce = false;
    [SerializeField] private bool canOpen = true;
    [SerializeField] private bool canClose = true;
    [SerializeField] private float journalCloseBuffer = 0.25f;
    [SerializeField] private bool enableTutorialMessage = true;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent<Note> noteAddedEvent;
    public UnityEvent<Journal> refreshJournalEvent;
    public UnityEvent clearJournalEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        if(noteAddedEvent == null)
        {
            noteAddedEvent = new UnityEvent<Note>();
        }

        if(refreshJournalEvent == null)
        {
            refreshJournalEvent = new UnityEvent<Journal>();
        }

        if(clearJournalEvent == null)
        {
            clearJournalEvent = new UnityEvent();
        }
    }

    /// <summary>
    /// Set whether the player has the Journal
    /// </summary>
    /// <param name="hasJournal">Whether the player has the Journal</param>
    public void SetHasJournal(bool hasJournal)
    {
        this.hasJournal = hasJournal;
    }

    /// <summary>
    /// Set whether the Journal is open or not
    /// </summary>
    /// <param name="journalOpen">Whether the Journal is open or not</param>
    public void SetJournalOpen(bool journalOpen)
    {
        this.journalOpen = journalOpen;
    }

    /// <summary>
    /// Set whether the Journal has been opened for the first time or not
    /// </summary>
    /// /// <param name="openedOnce">Whether the Journal has been opened for the first time or not</param>
    public void SetOpenedOnce(bool openedOnce)
    {
        this.openedOnce = openedOnce;
    }

    /// <summary>
    /// Set whether the Journal can be opened or not
    /// </summary>
    /// <param name="canOpen">Whether the Journal can be opened or not</param>
    public void SetCanOpen(bool canOpen)
    {
        this.canOpen = canOpen;
    }

    /// <summary>
    /// Set whether the player can close the Journal
    /// </summary>
    /// <param name="canClose">Whether the player can close the Journal</param>
    public void SetCanClose(bool canClose)
    {
        this.canClose = canClose;
    }

    /// <summary>
    /// Set the Journal close buffer
    /// </summary>
    /// <param name="journalCloseBuffer">The Journal close buffer</param>
    public void SetCloseBuffer(float journalCloseBuffer)
    {
        this.journalCloseBuffer = journalCloseBuffer;
    }

    /// <summary>
    /// Sets the tutorial message enabled value
    /// </summary>
    /// <param name="enableTutorialMessage">Whether or not to show the tutorial message</param>
    public void SetTutorialMessage(bool enableTutorialMessage)
    {
        this.enableTutorialMessage = enableTutorialMessage;
    }

    /// <summary>
    /// gets the tutorial message enabled value
    /// </summary>
    /// <returns>Whether to show the tutorial message</returns>
    public bool GetTutorialMessage()
    {
        return enableTutorialMessage;
    }

    /// <summary>
    /// Get whether the player has the Journal
    /// </summary>
    /// <returns>Whether the player has the Journal</returns>
    public bool GetHasJournal()
    {
        return hasJournal;
    }

    /// <summary>
    /// Get whether the player can open the Journal
    /// </summary>
    /// <returns>Whether the player can open the Journal</returns>
    public bool GetJournalOpen()
    {
        return journalOpen;
    }

    /// <summary>
    /// Set whether the Journal has been opened for the first time or not
    /// </summary>
    /// <returns>Whether the Journal has been opened for the first time or not</returns>
    public bool GetOpenedOnce()
    {
        return openedOnce;
    }

    /// <summary>
    /// Get whether the Journal can be opened or not
    /// </summary>
    /// <returns>Whether the Journal can be opened or not</returns>
    public bool GetCanOpenJournal()
    {
        return canOpen;
    }

    /// <summary>
    /// Get whether the player can close the Journal
    /// </summary>
    /// <returns>Whether the player can close the Journal</returns>
    public bool GetCanCloseJournal()
    {
        return canClose;
    }

    /// <summary>
    /// Get the Joural close buffer
    /// </summary>
    /// <returns>The Journal close buffer</returns>
    public float GetCloseBuffer()
    {
        return journalCloseBuffer;
    }

    /// <summary>
    /// Trigger any events that relate to a Note being added to the Journal
    /// </summary>
    /// <param name="noteToAdd"></param>
    public void AddNote(Note noteToAdd)
    {
        noteAddedEvent.Invoke(noteToAdd);
    }

    /// <summary>
    /// Trigger any events that relate to the Journal being refreshed
    /// </summary>
    /// <param name="journalToRefresh"></param>
    public void RefreshJournal(Journal journalToRefresh)
    {
        refreshJournalEvent.Invoke(journalToRefresh);
    }

    /// <summary>
    /// Trigger any events that relate to the Journal being cleared
    /// </summary>
    public void ClearJournal()
    {
        clearJournalEvent.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        hasJournal = false;
        journalOpen = false;
        openedOnce = false;
        canOpen = true;
        canClose = true;
        journalCloseBuffer = 0.25f;
        enableTutorialMessage = true;
    }
}

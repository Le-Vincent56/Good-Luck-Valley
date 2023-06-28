using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "JournalScriptableObject", menuName = "ScriptableObjects/Journal Event")]
public class JournalScriptableObj : ScriptableObject
{
    #region FIELDS
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
}

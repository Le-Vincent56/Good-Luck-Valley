using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Note : Interactable
{
    #region REFERENCES
    public Text textDisplay;
    private Journal journal;
    private EntryScrollview entryScrollview;
    private NoteNotification effectPanelNotification;
    #endregion

    #region FIELDS
    [SerializeField] private string noteTitle;
    [SerializeField] private string textValue;
    private bool noteAdded = false;
    #endregion

    #region PROPERTIES
    public string NoteTitle { get { return noteTitle; } set { noteTitle = value; } }
    public string TextValue { get { return textValue; } set { textValue = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();
        effectPanelNotification = GameObject.Find("NoteEffectPanel").GetComponent<NoteNotification>();

        remove = false;
    }

    public override void Interact()
    {
        // Add the note to the journal and trigger notification and sound effect
        if(!noteAdded)
        {
            journal.Notes.Add(this);
            effectPanelNotification.NotifQueue.Enqueue(this);
            journal.journalPageSound.Play();
            noteAdded = true;
        }

        // Finish interacting
        finishedInteracting = true;
        remove = true;
    }
}

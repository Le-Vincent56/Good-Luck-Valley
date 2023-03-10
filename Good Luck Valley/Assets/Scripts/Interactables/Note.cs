using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Note : Interactable
{
    public string noteTitle;
    public string textValue;
    private bool noteAdded = false;
    public Text textDisplay;
    private Journal journal;
    private EntryScrollview entryScrollview;
    private NoteNotification effectPanelNotification;

    // Start is called before the first frame update
    void Start()
    {
        remove = false;
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();
        effectPanelNotification = GameObject.Find("NoteEffectPanel").GetComponent<NoteNotification>();
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

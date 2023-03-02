using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Note : Interactable
{
    public string noteTitle;
    public string textValue;
    private Text textDisplay;
    private Journal journal;
    private NoteNotification effectPanelNotification;
    private float notificationTimeout;

    // Start is called before the first frame update
    void Start()
    {
        notificationTimeout = 3.0f;
        remove = false;
        //journal = GameObject.Find("Journal").GetComponent<Journal>();
        effectPanelNotification = GameObject.Find("NoteEffectPanel").GetComponent<NoteNotification>();
    }

    public override void Interact()
    {
        // Add the note to the journal
        //journal.Notes.Add(this);

        // Notify the player of the pickup and set the note title
        effectPanelNotification.TriggerNotif = true;
        effectPanelNotification.NoteTitle.text = noteTitle;
        effectPanelNotification.PrepareNotification();

        // Finish interacting
        finishedInteracting = true;
        remove = true;
    }
}

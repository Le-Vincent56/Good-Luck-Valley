using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteNotification : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private JournalScriptableObj journalEvent;
    private Note currentNote;
    private Image panelImage;
    private Text entryAddedText;
    private Text noteTitle;
    #endregion

    #region FIELDS
    [SerializeField] private float fadeInTimer = 1.0f;
    [SerializeField] private float notifTimer = 3.0f;
    [SerializeField] private float fadeOutTimer = 1.0f;
    [SerializeField] private bool continueToLingerTimer = false;
    [SerializeField] private bool continueToFadeOutTimer = false;
    [SerializeField] private bool fadeCompleted = false;
    [SerializeField] private bool notifInProgress = false;
    [SerializeField] private Queue<Note> notifQueue = new Queue<Note>();
    #endregion

    #region PROPERTIES
    public Text NoteTitle { get { return noteTitle; } set { noteTitle = value; } }
    public bool NotifInProgress { get { return notifInProgress; } set { notifInProgress = value; } }
    public Queue<Note> NotifQueue { get { return notifQueue; } set { notifQueue = value; } }

    private void OnEnable()
    {
        // Subscribe to Journal events
        journalEvent.noteAddedEvent.AddListener(EnqueueNote);
    }

    private void OnDisable()
    {
        // Unsubscribe to Journal events
        journalEvent.noteAddedEvent.RemoveListener(EnqueueNote);
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        panelImage = GameObject.Find("NoteEffectPanel").GetComponent<Image>();
        entryAddedText = GameObject.Find("EntryAddedText").GetComponent<Text>();
        noteTitle = GameObject.Find("NoteTitleText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there are any notifications in the Queue and none in progress
        if (notifQueue.Count > 0 && !notifInProgress)
        {
            // If so, set the currentNote as the first one in the Queue,
            // prepare the notification variables, and set a notifInProgress to true
            currentNote = notifQueue.Dequeue();
            PrepareNotification();
            notifInProgress = true;
        }

        // If a notification is in progress, resolve it
        if (notifInProgress)
        {
            ResolveNotif();
        }
    }

    /// <summary>
    /// Resolve Notification effects within the UI
    /// </summary>
    public void ResolveNotif()
    {
        // Fade the notification in
        if (fadeInTimer < 0.5)
        {
            // Add deltaTime to the fadeInTimer as well as the a value of the text and panel
            fadeInTimer += Time.deltaTime;
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 
                panelImage.color.a + (Time.deltaTime * 2));
            entryAddedText.color = new Color(entryAddedText.color.r, entryAddedText.color.g, entryAddedText.color.b, 
                entryAddedText.color.a + (Time.deltaTime * 2));
            noteTitle.color = new Color(noteTitle.color.r, noteTitle.color.g, noteTitle.color.b, 
                noteTitle.color.a + (Time.deltaTime * 2));

            // Set continueToLingerTimer to false for security
            continueToLingerTimer = false;
        } else continueToLingerTimer = true;

        // Leave text on the screen for a little
        if (notifTimer > 0 && continueToLingerTimer)
        {
            // Have the text and panel at full opacity until notifTimer runs out
            notifTimer -= Time.deltaTime;

            // Set continueToFadeOutTimer to false for security
            continueToFadeOutTimer = false;
        }
        else if (notifTimer <= 0)
        {
            // Once the notifTimer runs out, continue to the fade out timer
            continueToFadeOutTimer = true;
        }

        // Fade out text
        if (fadeOutTimer > 0 && continueToFadeOutTimer)
        {
            // Subtract deltaTime to the fadeInTimer as well as the a value of the text and panel
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 
                panelImage.color.a - Time.deltaTime);
            entryAddedText.color = new Color(entryAddedText.color.r, entryAddedText.color.g, entryAddedText.color.b, 
                entryAddedText.color.a - Time.deltaTime);
            noteTitle.color = new Color(noteTitle.color.r, noteTitle.color.g, noteTitle.color.b, 
                noteTitle.color.a - Time.deltaTime);
            fadeOutTimer -= Time.deltaTime;

            // Set fadeCompleted to false for security
            fadeCompleted = false;
        }
        else if (fadeOutTimer <= 0)
        {
            // Once the fadeOutTimer runs out, continue
            fadeCompleted = true;
        }

        // Once the fade is completed, set notifInProgress to false
        if (fadeCompleted)
        {
            notifInProgress = false;
        }
    }

    /// <summary>
    /// Prepare notifications to be shown in the UI
    /// </summary>
    public void PrepareNotification()
    {
        // Set timers and show text
        fadeInTimer = 0.0f;
        notifTimer = 3.0f;
        fadeOutTimer = 1.0f;

        // Set text
        noteTitle.text = currentNote.NoteTitle;

        // Reset booleans
        continueToLingerTimer = false;
        continueToFadeOutTimer = false;
        fadeCompleted = false;
    }

    #region EVENT FUNCTIONS
    /// <summary>
    /// Enqueue a Note to be notified of
    /// </summary>
    /// <param name="noteToEnqueue">The note to enqueue</param>
    public void EnqueueNote(Note noteToEnqueue)
    {
        notifQueue.Enqueue(noteToEnqueue);
    }
    #endregion
}

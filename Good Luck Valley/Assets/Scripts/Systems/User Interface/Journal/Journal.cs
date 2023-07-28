using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;

public class Journal : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private JournalScriptableObj journalEvent;
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private Text journalTutorialMessage;
    private Canvas journalUI;
    private Button pauseJournalButton;
    #endregion

    #region FIELDS
    [SerializeField] private List<Note> notes;
    [SerializeField] private bool menuOpen = false;
    [SerializeField] private bool hasJournal = false;
    [SerializeField] private bool hasOpenedOnce = false;
    [SerializeField] private bool openedFromKey = false;
    [SerializeField] private float journalCloseBuffer = 0.25f;
    [SerializeField] private bool canClose = false;
    [SerializeField] private bool showTutorialMessage = false;
    #endregion

    #region PROPERTIES
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    #endregion

    private void OnEnable()
    {
        journalEvent.noteAddedEvent.AddListener(AddNote);
        loadLevelEvent.startLoad.AddListener(DisableJournalOpen);
        loadLevelEvent.endLoad.AddListener(EnableJournalOpen);
    }

    private void OnDisable()
    {
        journalEvent.noteAddedEvent.RemoveListener(AddNote);
        loadLevelEvent.startLoad.RemoveListener(DisableJournalOpen);
        loadLevelEvent.endLoad.RemoveListener(EnableJournalOpen);
    }

    // Start is called before the first frame update
    void Start()
    {
        journalUI = GameObject.Find("JournalUI").GetComponent<Canvas>();
        pauseJournalButton = GameObject.Find("Journal Button").GetComponent<Button>();

        if (!journalEvent.GetTutorialMessage())
        {
            journalTutorialMessage.enabled = false;
            //journalTutorialMessage.GetComponent<Shadow>().enabled = false;
        }
        else
        {
            journalTutorialMessage.enabled = true;
            //.GetComponent<Shadow>().enabled = true;
        }

        // Set the journal menu to be invisible at first
        journalUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!journalEvent.GetHasJournal())
        {
            hasJournal = false;
            pauseJournalButton.interactable = false;
            Color jColor = pauseJournalButton.GetComponent<Image>().color;
            pauseJournalButton.GetComponent<Image>().color = new Color(jColor.r, jColor.g, jColor.b, 0.3f);
        } 
        else
        {
            hasJournal = true;
            pauseJournalButton.interactable = true;
            Color jColor = pauseJournalButton.GetComponent<Image>().color;
            pauseJournalButton.GetComponent<Image>().color = new Color(jColor.r, jColor.g, jColor.b, 1f);
        }

        // If the close buffer is set to above 0,
        // subtract by deltaTime
        if(journalCloseBuffer > 0 && !menuOpen)
        {
            journalCloseBuffer -= Time.deltaTime;
            journalEvent.SetCloseBuffer(journalCloseBuffer);
        }

        CheckJournalTutorial();
    }

    /// <summary>
    /// Open the Journal menu using a key input
    /// </summary>
    /// <param name="context">The context of the controller</param>
    public void OpenJournalKey(InputAction.CallbackContext context)
    {
        if (hasJournal && !menuOpen && journalEvent.GetCanOpenJournal())
        {
            // Pause the game
            pauseEvent.Pause();
            Time.timeScale = 0;
            openedFromKey = true;
            canClose = true;

            // Sort journal entries by indexes
            notes.Sort((a, b) => a.JournalIndex.CompareTo(b.JournalIndex));

            // Set the journal entries
            journalEvent.RefreshJournal(this);

            // Update so that it is no longer the first time opening
            if (!hasOpenedOnce)
            {
                hasOpenedOnce = true;
                journalEvent.SetOpenedOnce(true);
            }

            // Enable the journal UI and set menuOpen to true
            journalUI.enabled = true;
            menuOpen = true;
            journalCloseBuffer = 0.25f;
            journalEvent.SetJournalOpen(menuOpen);
            journalEvent.SetCanClose(canClose);
            journalEvent.SetCloseBuffer(journalCloseBuffer);

            // Play the open journal sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JournalOpen, transform.position);
        }
    }

    /// <summary>
    /// Close the Journal menu using a key input
    /// </summary>
    /// <param name="context">The context of the controller</param>
    public void CloseJournalKey(InputAction.CallbackContext context)
    {
        // Check if the menu is open
        if(menuOpen && canClose)
        {
            // Unpause the game - as long as the pause menu isn't open
            if(!pauseEvent.GetPauseMenuOpen())
            {
                pauseEvent.Unpause();
                Time.timeScale = 1f;
            }

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;
            journalEvent.SetJournalOpen(menuOpen);

            // Remove entries to prepare for sorting
            journalEvent.ClearJournal();

            // Play the close journal sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JournalClose, transform.position);
        }
    }

    /// <summary>
    /// Open the Journal using a function - for linking with settings button
    /// </summary>
    public void OpenJournal()
    {
        if(hasJournal && !menuOpen && journalEvent.GetCanOpenJournal())
        {
            // Sort journal entries by indexes
            notes.Sort((a, b) => a.JournalIndex.CompareTo(b.JournalIndex));

            // Set the journal entries
            journalEvent.RefreshJournal(this);

            // Update so that it is no longer the first time opening
            if (!hasOpenedOnce)
            {
                hasOpenedOnce = true;
                journalEvent.SetOpenedOnce(true);
            }

            journalUI.enabled = true;
            menuOpen = true;
            canClose = true;
            journalCloseBuffer = 0.25f;
            journalEvent.SetJournalOpen(menuOpen);
            journalEvent.SetCanClose(canClose);
            journalEvent.SetCloseBuffer(journalCloseBuffer);

            // Play the open journal sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JournalOpen, transform.position);
        }
    }

    /// <summary>
    /// Close the Journal using a function - for linking with an exit button
    /// </summary>
    public void CloseJournal()
    {
        // Check if the menu is open
        if (menuOpen && canClose)
        {
            // Unfreeze the game - only if the journal was opened from key
            if(openedFromKey && !pauseEvent.GetPauseMenuOpen())
            {
                pauseEvent.Unpause();
                Time.timeScale = 1f;
            }

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;
            journalEvent.SetJournalOpen(menuOpen);

            // Remove entries to prepare for sorting
            journalEvent.ClearJournal();

            // Play the close journal sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JournalClose, transform.position);
        }
    }

    /// <summary>
    /// Disable Journal functions relating to opening the Journal
    /// </summary>
    public void DisableJournalOpen()
    {
        journalEvent.SetCanOpen(false);
    }

    /// <summary>
    /// Enable Journal functions relating to opening the Journal
    /// </summary>
    public void EnableJournalOpen()
    {
        journalEvent.SetCanOpen(true);
    }

    /// <summary>
    /// Check if the journal tutorial needs to be enabled or disabled
    /// </summary>
    public void CheckJournalTutorial()
    {
        if (!journalEvent.GetTutorialMessage())
        {
            journalTutorialMessage.enabled = false;
        } else
        {
            journalTutorialMessage.enabled = true;
        }
    }

    #region EVENT FUNCTIONS
    /// <summary>
    /// Add a Note to the Journal
    /// </summary>
    /// <param name="noteToAdd">The note to add</param>
    public void AddNote(Note noteToAdd)
    {
        notes.Add(noteToAdd);
    }
    #endregion

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Clear the current notes
        notes.Clear();

        // Add notes according to their note data
        foreach(NoteData noteData in data.notes)
        {
            notes.Add(new Note(noteData.id, noteData.noteTitle, noteData.textValue, noteData.contentTitle, noteData.journalIndex, noteData.noteAdded));
        }

        // Set if the player has the journal according to data
        hasJournal = data.hasJournal;
        journalEvent.SetHasJournal(hasJournal);

        // Set if the player has opened the journal for the first time
        hasOpenedOnce = data.hasOpenedJournalOnce;
        journalEvent.SetOpenedOnce(hasOpenedOnce);

        // Set if the tutorial message needs to be shown
        showTutorialMessage = data.showJournalTutorial;
        journalEvent.SetTutorialMessage(showTutorialMessage);
    }

    public void SaveData(GameData data)
    {
        // Set the amount of notes collected
        data.numNotesCollected = notes.Count;

        // Save each note
        foreach (Note note in notes)
        {
            NoteData savedNote = new NoteData(note.ID, note.NoteTitle, note.TextValue, note.ContentsTitle, note.JournalIndex, note.NoteAdded);
            if (!data.notes.Contains(savedNote))
            {
                data.notes.Add(savedNote);
            }
        }

        // Save if the player has the journal or not
        data.hasJournal = hasJournal;

        // Save if the player has opened the journal or not
        data.hasOpenedJournalOnce = hasOpenedOnce;

        // Save if the tutorial message needs to be shown
        data.showJournalTutorial = showTutorialMessage;
    }
    #endregion
}

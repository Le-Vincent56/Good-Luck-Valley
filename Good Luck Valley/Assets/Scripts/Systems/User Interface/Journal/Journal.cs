using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class Journal : MonoBehaviour, IData
{
    #region REFERENCES
    private Canvas journalUI;
    private AudioSource journalPageSound;
    private Button pauseJournalButton;
    private EntryScrollview journalScrollview;
    #endregion

    #region FIELDS
    [SerializeField] private List<Note> notes;
    [SerializeField] private bool menuOpen = false;
    [SerializeField] private bool hasJournal = false;
    [SerializeField] private bool hasOpened = false;
    [SerializeField] bool openedFromKey = false;
    [SerializeField] float journalCloseBuffer = 0.25f;
    [SerializeField] bool canClose = false;
    #endregion

    #region PROPERTIES
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    public bool MenuOpen { get { return menuOpen; } set { menuOpen = value; } }
    public bool HasJournal { get { return hasJournal; } set { hasJournal = value;} }
    public bool HasOpened { get { return hasOpened; } set { hasOpened = value; } }
    public float CloseBuffer { get { return journalCloseBuffer; } set { journalCloseBuffer = value; } }
    public bool CanClose { get { return canClose; } set { canClose = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalUI = GameObject.Find("JournalUI").GetComponent<Canvas>();
        journalPageSound = GetComponent<AudioSource>();
        pauseJournalButton = GameObject.Find("Journal Button").GetComponent<Button>();
        journalScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();

        // Set the journal menu to be invisible at first
        journalUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasJournal)
        {
            pauseJournalButton.interactable = false;
        } else
        {
            pauseJournalButton.targetGraphic.color = pauseJournalButton.colors.selectedColor;
            pauseJournalButton.interactable = true;
        }

        // If the close buffer is set to above 0,
        // subtract by deltaTime
        if(journalCloseBuffer > 0 && !menuOpen)
        {
            journalCloseBuffer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Open the Journal menu using a key input
    /// </summary>
    /// <param name="context">The context of the controller</param>
    public void OpenJournalKey(InputAction.CallbackContext context)
    {
        if (hasJournal && !menuOpen)
        {
            // Pause the game
            EventManager.TriggerEvent("Pause", true);
            Time.timeScale = 0;
            openedFromKey = true;

            // Sort journal entries by indexes
            notes.Sort((a, b) => a.JournalIndex.CompareTo(b.JournalIndex));

            // Set the journal entries
            journalScrollview.SetEntries();

            // Update so that it is no longer the first time opening
            if (!hasOpened)
            {
                hasOpened = true;
            }

            // Enable the journal UI and set menuOpen to true
            journalUI.enabled = true;
            menuOpen = true;
            journalCloseBuffer = 0.25f;
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
            // Unpause the game
            EventManager.TriggerEvent("Pause", false);
            Time.timeScale = 1f;

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;

            // Remove entries to prepare for sorting
            journalScrollview.RemoveEntries();
        }
    }

    /// <summary>
    /// Open the Journal using a function - for linking with settings button
    /// </summary>
    public void OpenJournal()
    {
        if(hasJournal && !menuOpen)
        {
            // Sort journal entries by indexes
            notes.Sort((a, b) => a.JournalIndex.CompareTo(b.JournalIndex));

            // Set the journal entries
            journalScrollview.SetEntries();

            // Update so that it is no longer the first time opening
            if (!hasOpened)
            {
                hasOpened = true;
            }

            journalUI.enabled = true;
            menuOpen = true;
            journalCloseBuffer = 0.25f;
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
            if(openedFromKey)
            {
                Time.timeScale = 1f;
            }

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;

            // Remove entries to prepare for sorting
            journalScrollview.RemoveEntries();
        }
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        notes = data.notes;
        hasJournal = data.hasJournal;
    }

    public void SaveData(GameData data)
    {
        data.numNotesCollected = notes.Count;
        data.notes = notes;
        data.hasJournal = hasJournal;
    }
    #endregion
}

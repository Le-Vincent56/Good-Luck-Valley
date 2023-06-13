using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Journal : MonoBehaviour
{
    #region REFERENCES
    private Text panelText;
    private EntryScrollview entryScrollview;
    private Canvas journalUI;
    private AudioSource journalPageSound;
    private Button pauseJournalButton;
    private PlayerMovement playerMovement;
    private EntryScrollview journalScrollview;
    #endregion

    #region FIELDS
    [SerializeField] private List<Note> notes;
    [SerializeField] private bool menuOpen = false;
    [SerializeField] private bool hasJournal = false;
    [SerializeField] private bool hasOpened = false;
    [SerializeField] bool openedFromKey = false;
    [SerializeField] float journalCloseBuffer = 0.25f;
    #endregion

    #region PROPERTIES
    public AudioSource JournalPageSound { get { return journalPageSound; } set { journalPageSound = value; } }
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    public bool MenuOpen { get { return menuOpen; } set { menuOpen = value; } }
    public bool HasJournal { get { return hasJournal; } set { hasJournal = value;} }
    public bool HasOpened { get { return hasOpened; } set { hasOpened = value; } }
    public float CloseBuffer { get { return journalCloseBuffer; } set { journalCloseBuffer = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalUI = GameObject.Find("JournalUI").GetComponent<Canvas>();
        panelText = GameObject.Find("EntryText").GetComponent<Text>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();
        journalPageSound = GetComponent<AudioSource>();
        pauseJournalButton = GameObject.Find("Journal Button").GetComponent<Button>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
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
            // Set the journal entries
            journalScrollview.SetEntries();

            // Freeze player movement and set time to 0 - similar to a pause
            playerMovement.MoveInput = Vector2.zero;
            Time.timeScale = 0;
            openedFromKey = true;

            // Sort the journal by indexes
            Note tempNote = null;
            for (int i = 0; i <= notes.Count - 1; i++)
            {
                for (int j = i + 1; j < notes.Count; j++)
                {
                    // Loop through the first index and the index afterwards and compare the journal indexes
                    // so that the notes List is in ascending order
                    if (notes[i].JournalIndex > notes[j].JournalIndex)
                    {
                        // If a Note at the current index is larger than the note at the next index,
                        // then set tempNote to the current Note, set the current Note to the next Note,
                        // and set the next Note to the tempNote - switching around notes[i] and notes[j]
                        tempNote = notes[i];
                        notes[i] = notes[j];
                        notes[j] = tempNote;
                    }
                }
            }


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
        if(menuOpen)
        {
            // Unfreeze the game
            Time.timeScale = 1f;

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;
        }
    }

    /// <summary>
    /// Open the Journal using a function - for linking with settings button
    /// </summary>
    public void OpenJournal()
    {
        if(hasJournal && !menuOpen)
        {
            // Sort the journal by indexes
            Note tempNote = null;
            for(int i = 0; i <= notes.Count - 1; i++)
            {
                for(int j = i + 1; j < notes.Count; j++)
                {
                    // Loop through the first index and the index afterwards and compare the journal indexes
                    // so that the notes List is in ascending order
                    if (notes[i].JournalIndex > notes[j].JournalIndex)
                    {
                        // If a Note at the current index is larger than the note at the next index,
                        // then set tempNote to the current Note, set the current Note to the next Note,
                        // and set the next Note to the tempNote - switching around notes[i] and notes[j]
                        tempNote = notes[i];
                        notes[i] = notes[j];
                        notes[j] = tempNote;
                    }
                }
            }


            // Update so that it is no longer the first time opening
            if(!hasOpened)
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
        if (menuOpen)
        {
            // Unfreeze the game - only if the journal was opened from key
            if(openedFromKey)
            {
                Time.timeScale = 1f;
            }

            // Close the journal UI and set menuOpen to false
            journalUI.enabled = false;
            menuOpen = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    #region REFERENCES
    private Text panelText;
    private EntryScrollview entryScrollview;
    private Canvas journalUI;
    private AudioSource journalPageSound;
    private Button pauseJournalButton;
    #endregion

    #region FIELDS
    [SerializeField] private List<Note> notes;
    [SerializeField] private bool menuOpen = false;
    [SerializeField] private bool hasJournal = false;
    [SerializeField] private bool hasOpened = false;
    #endregion

    #region PROPERTIES
    public AudioSource JournalPageSound { get { return journalPageSound; } set { journalPageSound = value; } }
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    public bool MenuOpen { get { return menuOpen; } set { menuOpen = value; } }
    public bool HasJournal { get { return hasJournal; } set { hasJournal = value;} }
    public bool HasOpened { get { return hasOpened; } set { hasOpened = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalUI = GameObject.Find("JournalUI").GetComponent<Canvas>();
        panelText = GameObject.Find("EntryText").GetComponent<Text>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();
        journalPageSound = GetComponent<AudioSource>();
        pauseJournalButton = GameObject.Find("Journal Button").GetComponent<Button>();

        // Set the journal menu to be invisible at first
        journalUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasJournal)
        {
            pauseJournalButton.targetGraphic.color = pauseJournalButton.colors.disabledColor;
            pauseJournalButton.interactable = false;
        } else
        {
            pauseJournalButton.targetGraphic.color = pauseJournalButton.colors.normalColor;
            pauseJournalButton.interactable = true;
        }
    }

    public void ShowJournal()
    {
        if(hasJournal)
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
        }
    }

    public void CloseJournal()
    {
        journalUI.enabled = false;
        menuOpen = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private List<Note> notes;
    private Text panelText;
    private EntryScrollview entryScrollview;
    private Canvas journalUI;
    public bool menuOpen = false;
    #endregion

    #region PROPERTIES
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journalUI = GameObject.Find("JournalUI").GetComponent<Canvas>();
        panelText = GameObject.Find("EntryText").GetComponent<Text>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();

        // Set the journal menu to be invisible at first
        journalUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowJournal()
    {
        journalUI.enabled = true;
        menuOpen = true;
    }

    public void CloseJournal()
    {
        journalUI.enabled = false;
        menuOpen = false;
    }
}

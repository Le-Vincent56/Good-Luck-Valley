using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryButton : MonoBehaviour
{
    #region FIELDS
    private Text panelText;
    private EntryScrollview entryScrollview;
    private Journal journal;
    private Note associatedNote;
    #endregion

    #region PROPERTIES
    public Note Note { get { return associatedNote; } set { associatedNote = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        panelText = GameObject.Find("EntryText").GetComponent<Text>();
        entryScrollview = GameObject.Find("EntryPanel").GetComponent<EntryScrollview>();
    }

    public void RetrieveJournalEntry()
    {
        panelText.text = associatedNote.textValue;
    }
}

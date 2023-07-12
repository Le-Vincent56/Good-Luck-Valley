using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Note : Interactable, IData
{
    #region REFERENCES
    [SerializeField] private JournalScriptableObj journalEvent;
    public Text textDisplay;
    #endregion

    #region FIELDS
    [SerializeField] private string noteTitle;
    [SerializeField] private string textValue;
    [SerializeField] private string contentsTitle;
    [SerializeField] private int journalIndex = 0;
    [SerializeField] private bool noteAdded = false;
    #endregion

    #region PROPERTIES
    public string ID { get { return id; } set { id = value; } }
    public string NoteTitle { get { return noteTitle; } set { noteTitle = value; } }
    public string TextValue { get { return textValue; } set { textValue = value; } }
    public string ContentsTitle { get { return contentsTitle; } set { contentsTitle = value; } }
    public int JournalIndex { get { return journalIndex; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        remove = false;

        // If the note is already added, make it inactive in the scene
        if(noteAdded)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Collect the Note
    /// </summary>
    public override void Interact()
    {
        // Add the note to the journal and trigger notification and sound effect
        if(!noteAdded)
        {
            journalEvent.AddNote(this);
            noteAdded = true;
        }

        // Finish interacting
        finishedInteracting = true;
        remove = true;
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Get the data for all the notes that have been collected
        string currentLevel = SceneManager.GetActiveScene().name;

        data.levelData[currentLevel].notesCollected.TryGetValue(id, out noteAdded);
        Debug.Log("ID: " + id + ", Added: " + noteAdded);

        // Check if the note has been added
        if (noteAdded)
        {
            // Remove the note
            remove = true;
        } else
        {
            remove = false;
        }
    }

    public void SaveData(GameData data)
    {
        string currentLevel = SceneManager.GetActiveScene().name;

        // Check to see if data has the id of the note
        if (data.levelData[currentLevel].notesCollected.ContainsKey(id))
        {
            // If so, remove it
            data.levelData[currentLevel].notesCollected.Remove(id);
        }

        // Add the id and the current bool to make sure everything is up to date
        data.levelData[currentLevel].notesCollected.Add(id, noteAdded);
    }
    #endregion
}

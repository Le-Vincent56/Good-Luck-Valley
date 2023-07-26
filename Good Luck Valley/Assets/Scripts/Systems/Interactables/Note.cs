using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.TextCore.Text;

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
    [SerializeField] private bool progressesMusic;
    [SerializeField] private float progressLevel;
    #endregion

    #region PROPERTIES
    public string ID { get { return id; } set { id = value; } }
    public string NoteTitle { get { return noteTitle; } set { noteTitle = value; } }
    public string TextValue { get { return textValue; } set { textValue = value; } }
    public string ContentsTitle { get { return contentsTitle; } set { contentsTitle = value; } }
    public int JournalIndex { get { return journalIndex; } }
    public bool NoteAdded { get { return noteAdded; } }
    #endregion

    public Note(string id, string noteTitle, string textValue, string contentsTitle, int journalIndex, bool noteAdded)
    {
        this.id = id;
        this.noteTitle = noteTitle;
        this.textValue = textValue;
        this.contentsTitle = contentsTitle;
        this.journalIndex = journalIndex;
        this.noteAdded = noteAdded;
        remove = true;
    }

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
        if (!noteAdded)
        {
            // Set the object to invisible
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            // Add it to the journla
            journalEvent.AddNote(this);
            noteAdded = true;

            StartCoroutine(ProcessNote());
        }
    }

    private IEnumerator ProcessNote()
    {
        // Add the note to update music - only in level one
        if (AudioManager.Instance.CurrentArea == MusicArea.FOREST && AudioManager.Instance.CurrentForestLevel == ForestLevel.MAIN && progressesMusic)
        {
            yield return StartCoroutine(UpdateMusicLayer());
        }

        // Check if the player has the journal and if the note is the Journal Binding
        if (!journalEvent.GetHasJournal() && journalIndex == 0)
        {
            journalEvent.SetHasJournal(true);
        }

        // Finish interacting
        finishedInteracting = true;
        remove = true;

        // Play the note pickup sound
        if (!playedSound)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.NotePickup, transform.position);
            playedSound = true;
        }
    }

    private IEnumerator UpdateMusicLayer()
    {
        while(AudioManager.Instance.CurrentForestProgression <= progressLevel)
        {
            // Set the FMOD parameters
            AudioManager.Instance.SetForestProgress(AudioManager.Instance.CurrentForestProgression + (Time.deltaTime / 4f));

            if (AudioManager.Instance.CurrentForestProgression >= progressLevel)
            {
                // Round out the number
                AudioManager.Instance.SetForestProgress(Mathf.Floor(AudioManager.Instance.CurrentForestProgression));
            }

            // Allow other code to run
            yield return null;
        }
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Get the data for all the notes that have been collected
        string currentLevel = SceneManager.GetActiveScene().name;

        data.levelData[currentLevel].notesCollected.TryGetValue(id, out noteAdded);

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

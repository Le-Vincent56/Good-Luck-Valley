using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GameData
{
    #region FIELDS
    public long lastUpdated;

    #region GENERAL
    public float playtimeTotal;
    public string playtimeString;
    #endregion

    #region LEVEL
    public string levelName;
    public bool playCutscene;
    public SerializableDictionary<string, bool> notesCollected;
    #endregion

    #region PLAYER
    public Vector3 playerPosition;
    #endregion

    #region SHROOM
    public bool throwUnlocked;
    #endregion

    #region JOURNAL
    public int numNotesCollected;
    public List<Note> notes;
    public bool hasJournal;
    #endregion
    
    #endregion

    // Constructor will have default values for when the game starts when there's no data to load
    public GameData()
    {
        #region GENERAL
        playtimeTotal = 0;
        playtimeString = "0:00:00";
        #endregion

        #region LEVEL
        levelName = "Prologue";
        playCutscene = true;
        notesCollected = new SerializableDictionary<string, bool>();
        #endregion

        #region PLAYER
        playerPosition = new Vector3(-27.47f, 7.798f, 0f);
        #endregion

        #region SHROOM
        throwUnlocked = false;
        #endregion

        #region JOURNAL
        numNotesCollected = 0;
        notes = new List<Note>();
        hasJournal = false;
        #endregion
    }
}

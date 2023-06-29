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
    public SerializableDictionary<string, LevelData> levelData;
    public SerializableDictionary<string, bool> notesCollected;
    public string currentLevelName;
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
        levelData = new SerializableDictionary<string, LevelData>();
        levelData["Prologue"] = new LevelData(new Vector3(-27.46f, 7.85f, 0f));
        levelData["Level 1"] = new LevelData(new Vector3(-39.82f, 6.31f, 0f));
        notesCollected = new SerializableDictionary<string, bool>();
        currentLevelName = "Prologue";
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

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

        #region PROLOGUE
        levelData["Prologue"] = new LevelData();
        levelData["Prologue"].playerPosition = new Vector3(-27.46f, 7.85f, 0f);

        #region ASSETS
        // Spore vines
        levelData["Prologue"].assetsActive.Add("d36a4464-bcf2-4133-933e-edcc3f1c12e8", true);
        levelData["Prologue"].assetsActive.Add("cfcb25a9-c90c-4001-8b02-f3709a9e417d", true);
        levelData["Prologue"].assetsActive.Add("94f7a370-3732-44b5-8ef7-966b10c051e3", true);

        // Spore
        levelData["Prologue"].assetsActive.Add("62d1a4a9-3861-42b1-b0aa-06684d32d1f2", true);

        // Vine wall
        levelData["Prologue"].assetsActive.Add("f229ac6e-1474-412e-b4f5-d62b0e1f8dff", true);

        // Lotuses
        levelData["Prologue"].assetsActive.Add("3b83efc7-7f42-4edf-ba58-78df7032e497", true);
        levelData["Prologue"].assetsActive.Add("130aa8b9-595d-47f2-96bb-54818ee26ef9", true);
        #endregion
        #endregion

        #region LEVEL 1
        levelData["Level 1"] = new LevelData();
        levelData["Level 1"].playerPosition = new Vector3(-39.82f, 6.31f, 0f);
        #endregion

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

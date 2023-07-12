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

    #region PLAYER
    public bool isGrounded;
    public bool isJumping;
    public bool isFalling;
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
        InitializePrologue();
        InitializeLevelOne();
        currentLevelName = "Prologue";
        #endregion

        #region SHROOM
        throwUnlocked = false;
        #endregion

        #region JOURNAL
        numNotesCollected = 0;
        notes = new List<Note>();
        hasJournal = true;
        #endregion
    }

    public void InitializePrologue()
    {
        levelData["Prologue"] = new LevelData();
        levelData["Prologue"].levelPos = LEVELPOS.ENTER;
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
    }

    public void InitializeLevelOne()
    {
        levelData["Level 1"] = new LevelData();
        levelData["Level 1"].levelPos = LEVELPOS.ENTER;
        levelData["Level 1"].playerPosition = new Vector3(-39.82f, 6.31f, 0f);

        #region ASSETS
        // Notes
        levelData["Level 1"].notesCollected.Add("70958600-3752-40de-94a1-545b7409b81b", false);
        levelData["Level 1"].notesCollected.Add("6f1373e1-ac5f-45f5-825a-fe053b549b38", false);
        levelData["Level 1"].notesCollected.Add("3ccfd056-3a6d-4c19-ae95-636382b4d5c7", false);
        levelData["Level 1"].notesCollected.Add("816acec3-2d35-4019-bdd1-d73ff1902243", false);
        levelData["Level 1"].notesCollected.Add("c090b985-6611-4398-8794-995eccae309e", false);
        levelData["Level 1"].notesCollected.Add("aec1c7d8-92c4-4190-9d37-531aae631fe9", false);
        levelData["Level 1"].notesCollected.Add("4152cb73-9f26-473e-9e24-2dfccdb51401", false);
        levelData["Level 1"].notesCollected.Add("48bd0adf-8c03-4261-b6ff-d9824eaf9750", false);

        // Lotus
        levelData["Level 1"].assetsActive.Add("dd57ac64-ca88-4a0e-b33b-c9fd6faccb49", true);
        #endregion
    }
}

using HiveMind.Audio;
using UnityEngine;
using UnityEngine.Rendering;

namespace HiveMind.SaveData
{
    [System.Serializable]
    public struct NoteData
    {
        public string id;
        public string noteTitle;
        public string textValue;
        public string contentTitle;
        public int journalIndex;
        public bool noteAdded;
        public bool alreadyRead;

        public NoteData(string id, string noteTitle, string textValue, string contentTitle, int journalIndex, bool noteAdded, bool alreadyRead)
        {
            this.id = id;
            this.noteTitle = noteTitle;
            this.textValue = textValue;
            this.contentTitle = contentTitle;
            this.journalIndex = journalIndex;
            this.noteAdded = noteAdded;
            this.alreadyRead = alreadyRead;
        }
    }

    [System.Serializable]
    public class GameData
    {
        #region FIELDS
        public long lastUpdated;

        #region GENERAL
        public float playtimeTotal;
        public string playtimeString;
        #endregion

        #region POST-PROCESSING
        public VolumeProfile volumeProfile;
        #endregion

        #region PLAYER
        public bool isGrounded;
        public bool isJumping;
        public bool isFalling;
        public bool showFastFall;
        #endregion

        #region LEVEL
        public SerializableDictionary<string, LevelData> levelData;
        public string currentLevelName;
        #endregion

        #region MUSIC
        public ForestLevel currentForestLevel;
        public SerializableDictionary<string, float> forestLayers;
        #endregion

        #region SHROOM
        public bool throwUnlocked;
        public bool firstThrow;
        public bool firstBounce;
        public bool firstFull;
        public bool showQuickBounce;
        public bool firstWallBounce;
        #endregion

        #region JOURNAL
        public int totalNotes;
        public int numNotesCollected;
        public NoteData newestNote;
        public bool hasJournal;
        public bool hasOpenedJournalOnce;
        public bool showJournalTutorial;
        #endregion
        #endregion

        // Constructor will have default values for when the game starts when there's no data to load
        public GameData()
        {
            #region GENERAL
            playtimeTotal = 0;
            playtimeString = "0:00:00";
            showFastFall = true;
            #endregion

            #region LEVEL
            levelData = new SerializableDictionary<string, LevelData>();
            InitializePrologue();
            InitializeLevelOne();
            currentLevelName = "Prologue";
            #endregion

            #region MUSIC
            forestLayers = new SerializableDictionary<string, float>();
            #endregion

            #region SHROOM
            throwUnlocked = false;
            firstThrow = true;
            firstBounce = true;
            firstFull = false;
            showQuickBounce = true;
            firstWallBounce = true;
            #endregion

            #region JOURNAL
            totalNotes = 7;
            numNotesCollected = 0;
            hasJournal = false;
            hasOpenedJournalOnce = false;
            showJournalTutorial = true;
            #endregion
        }

        public void InitializePrologue()
        {
            levelData["Prologue"] = new LevelData();
            levelData["Prologue"].levelPos = LEVELPOS.DEFAULT;
            levelData["Prologue"].playerPosition = new Vector3(-27.46f, 7.85f, 0f);

            #region ASSETS
            // Spore vines
            levelData["Prologue"].assetsActive.Add("7d6b9b97-f377-4119-a443-468aeb2e7104", true);

            // Spore
            levelData["Prologue"].assetsActive.Add("62d1a4a9-3861-42b1-b0aa-06684d32d1f2", true);

            // Vine wall
            levelData["Prologue"].assetsActive.Add("f229ac6e-1474-412e-b4f5-d62b0e1f8dff", true);

            // Lotuses
            levelData["Prologue"].assetsActive.Add("3b83efc7-7f42-4edf-ba58-78df7032e497", true);
            levelData["Prologue"].assetsActive.Add("130aa8b9-595d-47f2-96bb-54818ee26ef9", true);

            // Notes
            levelData["Prologue"].totalNotes = 0;
            levelData["Prologue"].currentNotes = 0;
            #endregion
        }

        public void InitializeLevelOne()
        {
            levelData["Level 1"] = new LevelData();
            levelData["Level 1"].levelPos = LEVELPOS.ENTER;
            levelData["Level 1"].playerPosition = new Vector3(-39.82f, 6.31f, 0f);

            #region ASSETS
            // Notes
            levelData["Level 1"].notesCollected.Add("aecf6275-285b-43de-9998-08665fc9c8d6", false);
            levelData["Level 1"].notesCollected.Add("6f1373e1-ac5f-45f5-825a-fe053b549b38", false);
            levelData["Level 1"].notesCollected.Add("3ccfd056-3a6d-4c19-ae95-636382b4d5c7", false);
            levelData["Level 1"].notesCollected.Add("816acec3-2d35-4019-bdd1-d73ff1902243", false);
            levelData["Level 1"].notesCollected.Add("c090b985-6611-4398-8794-995eccae309e", false);
            levelData["Level 1"].notesCollected.Add("aec1c7d8-92c4-4190-9d37-531aae631fe9", false);
            levelData["Level 1"].notesCollected.Add("4152cb73-9f26-473e-9e24-2dfccdb51401", false);
            levelData["Level 1"].notesCollected.Add("48bd0adf-8c03-4261-b6ff-d9824eaf9750", false);

            // Lotus
            levelData["Level 1"].assetsActive.Add("dd57ac64-ca88-4a0e-b33b-c9fd6faccb49", true);

            // Vine Wall
            levelData["Level 1"].assetsActive.Add("c2827f9d-736d-425c-8f2d-1627e43195bd", true);

            // Notes
            levelData["Level 1"].totalNotes = 8;
            levelData["Level 1"].currentNotes = 0;
            #endregion
        }
    }
}

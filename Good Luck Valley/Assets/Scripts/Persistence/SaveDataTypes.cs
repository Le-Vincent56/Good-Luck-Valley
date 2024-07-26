using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData
    {
        public int Slot;
        public long LastUpdated;
        public string Name;
        public string CurrentLevelName;
        public PlayerSaveData playerSaveData;
        public JournalSaveData journalSaveData;
        public GlobalData globalData;
        public List<CollectibleSaveData> collectibleSaveDatas;

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Slot: {1}, Name: {Name}, Level: {CurrentLevelName}\n";
            finalString += playerSaveData.ToString() + "\n";
            finalString += journalSaveData.ToString() + "\n";
            finalString += globalData.ToString() + "\n";

            return finalString;
        }
    }

    [Serializable]
    public class PlayerSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public Vector3 position;

        public PlayerSaveData()
        {
            position = new Vector3(-122.7588f, 97.67071f, 0.0f);
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Position: {position}";

            return finalString;
        }
    }

    [Serializable]
    public class GlobalData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool unlockedThrow;
        public bool unlockedWallJump;

        public GlobalData()
        {
            unlockedThrow = false;
            unlockedWallJump = false;
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Unlocked Throw: {unlockedThrow}, ";
            finalString += $"Unlocked Wall Jump: {unlockedWallJump}";

            return finalString;
        }
    }

    [Serializable]
    public class CollectibleSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool collected;

        public CollectibleSaveData()
        {
            collected = false;
        }
    }

    [Serializable]
    public class JournalSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool unlocked;
        public int progressingIndex;
        public int lastOpenedIndex;
        public int notesCollectedNum;
        public int journalEntriesUnlocked;

        public JournalSaveData()
        {
            unlocked = false;
            progressingIndex = 0;
            lastOpenedIndex = 0;
            notesCollectedNum = 0;
            journalEntriesUnlocked = 0;
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Unlocked Journal: {unlocked}, ";
            finalString += $"Progressing Index: {progressingIndex}, ";
            finalString += $"Last Opened Index: {lastOpenedIndex}, ";
            finalString += $"Notes Collected: {notesCollectedNum}, ";
            finalString += $"Entries Unlocked: {journalEntriesUnlocked}";

            return finalString;
        }
    }
}

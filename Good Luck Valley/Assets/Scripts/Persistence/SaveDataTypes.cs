using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData
    {
        public long LastUpdated;
        public string Name;
        public string CurrentLevelName;
        public PlayerSaveData playerSaveData;
        public JournalSaveData journalSaveData;
        public GlobalData globalData;
        public List<CollectibleSaveData> collectibleSaveDatas;
    }

    [Serializable]
    public class PlayerSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public Vector3 position;

        public PlayerSaveData()
        {
            position = new Vector3(-21.242f, -46.16f, 0.0f);
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
    }
}

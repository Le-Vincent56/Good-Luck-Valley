using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData
    {
        public int Slot;
        public long LastUpdated;
        public string Name;
        public PlayerData PlayerData;
        public JournalData JournalData;
        public List<CollectibleSaveData> CollectibleDatas;

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Slot: {Slot}, Name: {Name}";
            finalString += $"Player Data: {PlayerData}";

            return finalString;
        }
    }

    [Serializable]
    public class PlayerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public int LevelIndex;
        public Vector2 Position;

        public PlayerData()
        {
            LevelIndex = 3;
            Position = new Vector3(6.95f, -2.7f, 0.0f);
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"\n\tPosition: {Position}";

            return finalString;
        }
    }

    [Serializable]
    public class JournalData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool Unlocked;
        public List<int> EntriesUnlocked;

        public JournalData()
        {
            Unlocked = false;
            EntriesUnlocked = new List<int>();
        }
    }

    [Serializable]
    public class CollectibleSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool Collected;

        public CollectibleSaveData()
        {
            Collected = false;
        }
    }
}

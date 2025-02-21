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
        public List<CameraData> CameraDatas;
        public List<TimelineData> TimelineDatas;

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
            LevelIndex = 2;
            Position = new Vector3(-139.03f, 0.13f, 0.0f);
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"\n\tPosition: {Position}";

            return finalString;
        }
    }

    [Serializable]
    public class CameraData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public int Priority;
        public float OrthographicSize;
        public Vector3 Offset;
        public Vector3 Position;
        public float PathPosition;
        public Vector3 Damping;

        public CameraData()
        {
            Priority = 0;
            OrthographicSize = 0.0f;
            Offset = Vector3.zero;
            Position = Vector3.zero;
            PathPosition = 0.0f;
            Damping = Vector3.zero;
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

    [Serializable]
    public class TimelineData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool Played;

        public TimelineData()
        {
            Played = false;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData : Data
    {
        public int Slot;
        public long LastUpdated;
        public PlayerData PlayerData;
        public JournalData JournalData;
        public List<CollectibleSaveData> CollectibleDatas;
        public List<CameraData> CameraDatas;
        public List<ActivateableTriggerData> ActivateableTriggerDatas;
        public List<TimelineData> TimelineDatas;
    }

    [Serializable]
    public class PlayerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public int LevelIndex;
        public Vector2 Position;

        public PlayerData()
        {
            LevelIndex = 10;
            Position = new Vector3(-340.84f, 133f, 0.0f);
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
    public class ActivateableTriggerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool Active;

        public ActivateableTriggerData()
        {
            Active = false;
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

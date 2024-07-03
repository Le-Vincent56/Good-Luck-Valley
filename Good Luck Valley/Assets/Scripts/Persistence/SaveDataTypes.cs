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
        public GlobalData globalData;
        public List<CollectibleSaveData> collectibleSaveDatas;

        public override string ToString()
        {
            return $"Player: {playerSaveData}\n";
        }
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

        public override string ToString()
        {
            return $"Position: {position}";
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
}

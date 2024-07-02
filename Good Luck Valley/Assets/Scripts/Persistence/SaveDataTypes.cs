using System;
using System.Collections;
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
        public SettingsData settingsData;
        public PlayerSaveData playerSaveData;
        public MushroomSaveData mushroomSaveData;
        public List<CollectibleSaveData> collectibleSaveDatas;
    }

    [Serializable]
    public class SettingsData
    {

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
    public class MushroomSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool unlockedThrow;
        public bool unlockedWallJump;

        public MushroomSaveData()
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

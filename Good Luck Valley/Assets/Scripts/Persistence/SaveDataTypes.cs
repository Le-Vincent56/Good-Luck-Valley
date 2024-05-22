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
        public bool isFacingRight;
        public string previousState;
        public string currentState;

        public PlayerSaveData()
        {
            position = new Vector3(22.64f, 15.86f, 0.0f);
            isFacingRight = true;
        }
    }
}

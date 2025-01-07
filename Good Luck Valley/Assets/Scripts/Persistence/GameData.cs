using System;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData
    {
        public int Slot;
        public long LastUpdated;
        public string Name;
        public int SceneGroupIndex;
        public PlayerData PlayerData;

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Slot: {Slot}, Name: {Name}, Scene Group Index: {SceneGroupIndex}\n";
            finalString += $"Player Data: {PlayerData}";

            return finalString;
        }
    }

    [Serializable]
    public class PlayerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public Vector2 Position;

        public PlayerData()
        {
            Position = new Vector3(6.95f, -2.7f, 0.0f);
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"\n\tPosition: {Position}";

            return finalString;
        }
    }
}

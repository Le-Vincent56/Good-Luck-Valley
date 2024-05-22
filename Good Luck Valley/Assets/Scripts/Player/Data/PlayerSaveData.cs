using GoodLuckValley.Persistence;
using GoodLuckValley.Player.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player
{
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
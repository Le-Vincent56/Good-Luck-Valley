using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Checkpoints
{
    public class EndLevelCheckpoint : MonoBehaviour
    {
        [SerializeField] private GameEvent onGameEnd;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Return if the trigger is not entered by the player
            if (collision.gameObject.tag != "Player") return;

            onGameEnd.Raise(this, null);
        }
    }
}
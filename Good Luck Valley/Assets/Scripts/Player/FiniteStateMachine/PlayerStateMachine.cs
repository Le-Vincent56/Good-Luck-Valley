using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerStateMachine
    {
        #region PROPERTIES
        public PlayerState CurrentState { get; private set; }
        #endregion

        public void Initialize(PlayerState startingState)
        {
            // Set the starting state and enter it
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(PlayerState newState)
        {
            // Exit the current state
            CurrentState.Exit();

            // Set the new state and enter it
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}

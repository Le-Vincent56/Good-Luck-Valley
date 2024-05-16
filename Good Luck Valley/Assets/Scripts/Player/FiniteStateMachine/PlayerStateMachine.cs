using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerStateMachine
    {
        #region PROPERTIES
        public PlayerState CurrentState { get; private set; }
        public PlayerState PreviousState { get; private set; }
        #endregion

        /// <summary>
        /// Initialize the State Machine with a starting state
        /// </summary>
        /// <param name="startingState"></param>
        public void Initialize(PlayerState startingState)
        {
            // Set the starting state and enter it
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>
        ///  Change the Current State of the Player
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(PlayerState newState)
        {
            // Set last state
            PreviousState = CurrentState;

            // Exit the current state
            CurrentState.Exit();

            // Set the new state and enter it
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine
{
    public class MushroomStateMachine
    {
        #region PROPERTIES
        public MushroomState CurrentState { get; private set; }
        public MushroomState PreviousState { get; private set; }
        #endregion

        /// <summary>
        /// Initialize the State Machine with a starting state
        /// </summary>
        /// <param name="startingState"></param>
        public void Initialize(MushroomState startingState)
        {
            // Set the starting state and enter it
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>
        ///  Change the Current State of the Player
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(MushroomState newState)
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

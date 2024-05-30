using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachineOld
{
    public class MushroomStateMachineOld
    {
        #region PROPERTIES
        public MushroomStateOld CurrentState { get; private set; }
        public MushroomStateOld PreviousState { get; private set; }
        #endregion

        /// <summary>
        /// Initialize the State Machine with a starting state
        /// </summary>
        /// <param name="startingState"></param>
        public void Initialize(MushroomStateOld startingState)
        {
            // Set the starting state and enter it
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>
        ///  Change the Current State of the Player
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(MushroomStateOld newState)
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

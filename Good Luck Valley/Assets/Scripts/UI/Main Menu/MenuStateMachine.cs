using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuStateMachine
    {
        #region PROPERTIES
        public MenuState PreviousState { get; private set; }
        public MenuState CurrentState { get; private set; }
        #endregion

        public void Initialize(MenuState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(MenuState newState)
        {
            // Set previous state
            PreviousState = CurrentState;

            // Exit the current state
            CurrentState.Exit();

            // Set the new state and enter it
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
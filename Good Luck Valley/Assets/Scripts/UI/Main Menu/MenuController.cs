using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuController : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private string previousState;
        [SerializeField] private string currentState;
        [SerializeField] private List<GameObject> screens = new List<GameObject>();

        private int stateChange = 0;
        #endregion

        #region PROPERTIES
        public MenuStateMachine StateMachine { get; private set; }
        public MenuInitialState InitialState { get; private set; }
        public MenuMainState MainState { get; private set; }
        #endregion

        private void Awake()
        {
            // Create a new state machine
            StateMachine = new MenuStateMachine();

            // Set states
            InitialState = new MenuInitialState(this, StateMachine, true);
        }

        private void Start()
        {
            StateMachine.Initialize(InitialState);
        }

        public void Show(int state)
        {
            switch(state)
            {
                case 0:
                    if(InitialState.FadeInOut)
                    {

                    }
                    break;
            }
        }

        public void Hide(int state)
        {

        }

        /// <summary>
        /// Check the current Menu State
        /// </summary>
        /// <returns></returns>
        public int CheckStateChange()
        {
            return stateChange;
        }

        /// <summary>
        /// Set the Menu State
        /// </summary>
        /// <param name="state">The Menu State to change to</param>
        public void SetState(int state)
        {
            stateChange = state;
        }
    }
}
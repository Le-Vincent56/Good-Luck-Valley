using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public MenuLoadState LoadState { get; private set; }
        #endregion

        private void Awake()
        {
            // Create a new state machine
            StateMachine = new MenuStateMachine();

            // Set states
            InitialState = new MenuInitialState(this, StateMachine, true, screens[0]);
            MainState = new MenuMainState(this, StateMachine, true, screens[1], screens[2]);
            LoadState = new MenuLoadState(this, StateMachine, true, screens[3], GetComponentInChildren<LoadController>());

            // Set state data
            InitialState.InstantiateUILists();
            MainState.InstantiateUILists();
            LoadState.InstantiateUILists();

            // Set each object to be false
            for(int i = 1; i < screens.Count; i++)
            {
                screens[i].SetActive(false);
            }
        }

        private void Start()
        {
            // Update UI
            UpdateUIFromSaveData();

            // Initialize the State Machine
            StateMachine.Initialize(InitialState);
        }

        private void Update()
        {
            // Update the logic of the current state
            StateMachine.CurrentState.LogicUpdate();

            // Check and set states
            if (StateMachine.PreviousState != null)
                previousState = StateMachine.PreviousState.ToString().Substring(27);
            currentState = StateMachine.CurrentState.ToString().Substring(27);
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

        /// <summary>
        /// Leave the Initial Menu State
        /// </summary>
        public void LeaveInitial()
        {
            // Only allow state change due to any key when in the initial state
            if (StateMachine.CurrentState is not MenuInitialState) return;
            stateChange = 1;
        }

        /// <summary>
        /// Update the Main Menu UI according to save data
        /// </summary>
        public void UpdateUIFromSaveData()
        {
            // Get save count
            if (SaveLoadSystem.Instance.GetSaveCount() != 0)
            {
                // Set Continue/Load Game
                MainState.UsingAlt = true;
                MainState.UpdateObject();
            } else
            {
                // Set New Game
                MainState.UsingAlt = false;
                MainState.UpdateObject();
            }
        }
    }
}
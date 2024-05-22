using GoodLuckValley.Persistence;
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
        public MenuLoadState LoadState { get; private set; }
        #endregion

        private void Awake()
        {
            // Create a new state machine
            StateMachine = new MenuStateMachine();

            // Set states
            InitialState = new MenuInitialState(this, StateMachine, true, screens[0]);
            MainState = new MenuMainState(this, StateMachine, true, screens[1]);
            LoadState = new MenuLoadState(this, StateMachine, true, screens[3]);

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
            StateMachine.Initialize(InitialState);

            // Get save count to see if to use Play or Continue/enable Load Saves
            if(SaveLoadSystem.Instance.GetSaveCount() != 0)
            {
                MainState.UIObject = screens[2];
                MainState.InstantiateUILists();
            }
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
    }
}
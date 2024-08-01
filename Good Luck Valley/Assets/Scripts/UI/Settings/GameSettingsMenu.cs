using GoodLuckValley.Events;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using GoodLuckValley.UI.Settings.States;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Settings
{
    [Serializable]
    public struct Exclusions
    {
        public List<Graphic> Objects;
    }

    public class GameSettingsMenu : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onSetMenuInput;
        [SerializeField] private GameEvent onExitGameSettings;

        [Header("Fields")]
        [SerializeField] private int state;
        [SerializeField] private GameObject[] screens = new GameObject[4];
        [SerializeField] private Exclusions[] exclusions = new Exclusions[4];
        [SerializeField] private MenuCursor[] cursors = new MenuCursor[4];

        private StateMachine stateMachine;

        public int EMPTY { get => -1; }
        public int MAIN { get => 0; }
        public int AUDIO { get => 1; }
        public int VIDEO { get => 2; }
        public int CONTROLS { get => 3; }

        private void Awake()
        {
            // Initialize the state machine
            stateMachine = new StateMachine();

            // Initialize states
            EmptyMenuState emptyState = new EmptyMenuState(this, stateMachine, false, exclusions[0], null, null);
            SettingsMenuState mainState = new SettingsMenuState(this, stateMachine, true, exclusions[MAIN], screens[MAIN], cursors[MAIN]);
            AudioMenuState audioState = new AudioMenuState(this, stateMachine, true, exclusions[AUDIO], screens[AUDIO], cursors[AUDIO]);
            VideoMenuState videoState = new VideoMenuState(this, stateMachine, true, exclusions[VIDEO], screens[VIDEO], cursors[VIDEO]);
            ControlsMenuState controlsState = new ControlsMenuState(this, stateMachine, true, exclusions[CONTROLS], screens[CONTROLS], cursors[CONTROLS]);

            // Set state transitions
            At(emptyState, mainState, new FuncPredicate(() => state == MAIN));

            At(mainState, emptyState, new FuncPredicate(() => state == EMPTY));
            At(mainState, audioState, new FuncPredicate(() => state == AUDIO));
            At(mainState, videoState, new FuncPredicate(() => state == VIDEO));
            At(mainState, controlsState, new FuncPredicate(() => state == CONTROLS));
            
            At(audioState, mainState, new FuncPredicate(() => state == MAIN));
            At(videoState, mainState, new FuncPredicate(() => state == MAIN));
            At(controlsState, mainState, new FuncPredicate(() => state == MAIN));

            // Set the main state
            state = EMPTY;

            stateMachine.SetState(emptyState);
        }

        private void Update()
        {
            // Update the state machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Update the state machine
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Set the state of the Title Screen
        /// </summary>
        /// <param name="state"></param>
        public void SetState(int state) => this.state = state;

        /// <summary>
        /// Handle back input
        /// </summary>
        /// <param name="started"></param>
        public void Back(bool started)
        {
            // Exit case - if the key has not been lifted yet
            if (started) return;

            CloseSettings();
        }

        public void OpenSettings(Component sender, object data)
        {
            // Enable menu input
            onSetMenuInput.Raise(this, null);

            state = MAIN;
        }

        public void CloseSettings()
        {
            // Set the state to empty
            state = EMPTY;

            // Show the pause menu
            onExitGameSettings.Raise(this, null);
        }
    }
}
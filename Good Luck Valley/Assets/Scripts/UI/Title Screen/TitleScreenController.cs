using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI.TitleScreen.States;
using GoodLuckValley.UI.Menus;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GoodLuckValley.UI.TitleScreen.Settings;

namespace GoodLuckValley.UI.TitleScreen
{
    [Serializable]
    public struct Exclusions
    {
        public List<Graphic> Objects;
    }

    public class TitleScreenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MenuInputReader inputReader;
        [SerializeField] private TitleBackgroundFade backgroundFade;

        [Header("Fields")]
        [SerializeField] private int state;
        [SerializeField] private bool tryingToChangeState;
        [SerializeField] private bool changingState;
        [SerializeField] private MenuCursor[] cursors = new MenuCursor[6];
        [SerializeField] private GameObject[] screens = new GameObject[7];
        [SerializeField] private Exclusions[] exclusions = new Exclusions[7];


        private StateMachine stateMachine;

        public int INIT { get => 0; }
        public int MAIN { get => 1; }
        public int START { get => 2; }
        public int SETTINGS { get => 3; }
        public int AUDIO { get => 4; }
        public int VIDEO { get => 5; }
        public int CONTROLS { get => 6; }

        public int CURSOR_MAIN { get => 0; }
        public int CURSOR_START { get => 1; }
        public int CURSOR_SETTINGS_MAIN { get => 2; }
        public int CURSOR_SETTINGS_AUDIO { get => 3;}
        public int CURSOR_SETTINGS_VIDEO { get => 4; }
        public int CURSOR_SETTINGS_CONTROLS { get => 5; }

        private void Awake()
        {
            // Set time scale to normal time
            Time.timeScale = 1.0f;

            // Set the state to the initialize screen
            state = INIT;

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Construct states
            InitialMenuState initialState = new InitialMenuState(this, stateMachine, true, exclusions[INIT], screens[INIT], null);
            MainMenuState mainState = new MainMenuState(this, stateMachine, true, exclusions[MAIN], screens[MAIN], cursors[CURSOR_MAIN], backgroundFade);
            StartMenuState startState = new StartMenuState(this, stateMachine, true, exclusions[START], screens[START], cursors[CURSOR_START], backgroundFade);
            SettingsMenuState settingsState = new SettingsMenuState(this, stateMachine, true, exclusions[SETTINGS], screens[SETTINGS], cursors[CURSOR_SETTINGS_MAIN], backgroundFade);
            AudioMenuState audioState = new AudioMenuState(this, stateMachine, true, exclusions[AUDIO], screens[AUDIO], cursors[CURSOR_SETTINGS_AUDIO]);
            VideoMenuState videoState = new VideoMenuState(this, stateMachine, true, exclusions[VIDEO], screens[VIDEO], cursors[CURSOR_SETTINGS_VIDEO]);
            ControlsMenuState controlsState = new ControlsMenuState(this, stateMachine, true, exclusions[CONTROLS],screens[CONTROLS], cursors[CURSOR_SETTINGS_CONTROLS]);

            // Exclude certain elements
            //videoState.AddExcludeds(excludedGraphics);

            // Set state transitions
            At(initialState, mainState, new FuncPredicate(() => state == MAIN));

            At(mainState, startState, new FuncPredicate(() => state == START));
            At(mainState, settingsState, new FuncPredicate(() => state == SETTINGS));

            At(startState, mainState, new FuncPredicate(() => state == MAIN));

            At(settingsState, mainState, new FuncPredicate(() => state == MAIN));
            At(settingsState, audioState, new FuncPredicate(() => state == AUDIO));
            At(settingsState, videoState, new FuncPredicate(() => state == VIDEO));
            At(settingsState, controlsState, new FuncPredicate(() => state == CONTROLS));

            At(audioState, settingsState, new FuncPredicate(() => state == SETTINGS));
            At(videoState, settingsState, new FuncPredicate(() => state == SETTINGS));
            At(controlsState, settingsState, new FuncPredicate(() => state == SETTINGS));

            // Set the initial state
            stateMachine.SetState(initialState);
        }

        private void OnEnable()
        {
            inputReader.Start += InitializeMenu;
        }

        private void OnDisable()
        {
            inputReader.Start -= InitializeMenu;
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
        /// Initialize the Main Menu
        /// </summary>
        /// <param name="started"></param>
        private void InitializeMenu(bool started)
        {
            // Exit case - if the button hasn't been lifted yet
            if (started) return;

            // Exit case - if state isn't init
            if (state != INIT) return;

            // Set the state to the main menu state
            state = MAIN;

            // Switch action maps
            inputReader.SwitchToUIActionMap();
        }

        /// <summary>
        /// Set the state of the Title Screen
        /// </summary>
        /// <param name="state"></param>
        public void SetState(int state) => this.state = state;
    }
}
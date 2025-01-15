using GoodLuckValley.Input;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.UI.MainMenu.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GoodLuckValley.UI.MainMenu.OptionMenus;
using Sirenix.OdinInspector;
using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu
{
    public class MainMenuController : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIInputReader inputReader;
        [SerializeField] private Image darkerBackground;

        [Header("States")]
        [SerializeField] private int currentState;
        [SerializeField] private List<CanvasGroup> screens;
        [SerializeField] private List<IOptionMenu> optionMenus;
        private StateMachine stateMachine;

        public int OPEN => 0;
        public int INITIAL => 1;
        public int START => 2;
        public int SETTINGS => 3;
        public int AUDIO => 4;
        public int VIDEO => 5;
        public int CONTROLS => 6;

        private void Awake()
        {
            // Get the screens
            optionMenus = GetComponentsInChildren<IOptionMenu>().ToList();

            // Set the current state to 0
            currentState = 0;

            // Set up the State Machine
            SetupStateMachine();

            // Set the UI input reader
            inputReader.Set();

            // Register as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void OnEnable()
        {
            inputReader.Submit += OpenMainMenu;
        }

        private void OnDisable()
        {
            inputReader.Submit -= OpenMainMenu;
        }

        private void Update()
        {
            // Update the State Machine
            stateMachine.Update();
        }

        /// <summary>
        /// Set up the Main Menu Controller's State Machine
        /// </summary>
        private void SetupStateMachine()
        {
            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Create states
            OpenMenuState openState = new OpenMenuState(this, screens[OPEN], darkerBackground, optionMenus[OPEN], 0.5f);
            InitialMenuState initialState = new InitialMenuState(this, screens[INITIAL], darkerBackground, optionMenus[INITIAL], 0.5f);
            StartMenuState startState = new StartMenuState(this, screens[START], darkerBackground, optionMenus[START], 0.5f);
            SettingsMenuState settingsState = new SettingsMenuState(this, screens[SETTINGS], darkerBackground, optionMenus[SETTINGS], 0.5f);
            AudioMenuState audioState = new AudioMenuState(this, screens[AUDIO], darkerBackground, optionMenus[AUDIO], 0.5f);
            VideoMenuState videoState = new VideoMenuState(this, screens[VIDEO], darkerBackground, optionMenus[VIDEO], 0.5f);
            ControlsMenuState controlsState = new ControlsMenuState(this, screens[CONTROLS], darkerBackground, optionMenus[CONTROLS], 0.5f);

            // Define state transitions
            stateMachine.At(openState, initialState, new FuncPredicate(() => currentState == INITIAL));

            stateMachine.At(initialState, startState, new FuncPredicate(() => currentState == START));
            stateMachine.At(initialState, settingsState, new FuncPredicate(() => currentState == SETTINGS));

            stateMachine.At(startState, initialState, new FuncPredicate(() => currentState == INITIAL));

            stateMachine.At(settingsState, initialState, new FuncPredicate(() => currentState == INITIAL));
            stateMachine.At(settingsState, audioState, new FuncPredicate(() => currentState == AUDIO));
            stateMachine.At(settingsState, videoState, new FuncPredicate(() => currentState == VIDEO));
            stateMachine.At(settingsState, controlsState, new FuncPredicate(() => currentState == CONTROLS));

            stateMachine.At(audioState, settingsState, new FuncPredicate(() => currentState == SETTINGS));

            stateMachine.At(videoState, settingsState, new FuncPredicate(() => currentState == SETTINGS));

            stateMachine.At(controlsState, settingsState, new FuncPredicate(() => currentState == SETTINGS));

            // Set the initial state
            stateMachine.SetState(openState);
        }

        /// <summary>
        /// Set the current state of the Main Menu Controller
        /// </summary>
        public void SetState(int state) => currentState = state;

        /// <summary>
        /// Open the Main Menu by setting the initial state
        /// </summary>
        private void OpenMainMenu(bool started)
        {
            // Exit case - the button is being lifted
            if (!started) return;

            // Exit case - not in the Open State
            if (currentState != OPEN) return;

            // Set the initial state
            SetState(INITIAL);
        }
    }
}

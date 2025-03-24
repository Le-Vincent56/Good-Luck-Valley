using GoodLuckValley.Input;
using GoodLuckValley.Architecture.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.UI.Menus.OptionMenus;
using Sirenix.OdinInspector;
using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine.UI;
using GoodLuckValley.UI.Menus.Main.States;
using GoodLuckValley.Audio;
using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Input;

namespace GoodLuckValley.UI.Menus.Main
{
    public class MainMenuController : SerializedMonoBehaviour, ITransmutableInputUI
    {
        [Header("References")]
        [SerializeField] private UIInputReader inputReader;
        [SerializeField] private Image darkerBackground;
        private ControlSchemeDetector inputDetector;
        private SaveLoadSystem saveLoadSystem;

        [Header("States")]
        [SerializeField] private int currentState;
        [SerializeField] private List<CanvasGroup> screens;
        [SerializeField] private List<IOptionMenu> optionMenus;
        [SerializeField] private List<IMenuController> menuControllers;
        private IMenuController currentMenuController;
        private StateMachine stateMachine;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.State menuState;
        [SerializeField] private AK.Wwise.Event enterInitialEvent;
        [SerializeField] private AK.Wwise.Event backtrackEvent;

        public int OPEN => 0;
        public int INITIAL => 1;
        public int START => 2;
        public int SETTINGS => 3;
        public int AUDIO => 4;
        public int VIDEO => 5;
        public int CONTROLS => 6;

        private void Awake()
        {
            optionMenus = new List<IOptionMenu>();
            menuControllers = new List<IMenuController>();

            // Get the screens
            GetComponentsInChildren(optionMenus);
            GetComponentsInChildren(menuControllers);

            // Set the current state to 0
            currentState = 0;

            // Set up the State Machine
            SetupStateMachine();

            // Set the UI input reader
            inputReader.Set();

            // Register as a service
            ServiceLocator.ForSceneOf(this).Register(this);

            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
            inputDetector = ServiceLocator.Global.Get<ControlSchemeDetector>();
        }

        private void OnEnable()
        {
            inputReader.Start += OpenMainMenu;
            inputReader.Cancel += Backtrack;
            saveLoadSystem.Release += Cleanup;
            saveLoadSystem.SettingsSet += PlayMenuMusic;

            inputDetector.Register(this);
        }

        private void OnDisable()
        {
            inputReader.Start -= OpenMainMenu;
            inputReader.Cancel -= Backtrack;

            inputDetector.Deregister(this);

            Cleanup();
        }

        private void Update()
        {
            // Update the State Machine
            stateMachine.Update();
        }

        /// <summary>
        /// Cleanup by unsubscribing from events from the Save Load System
        /// </summary>
        private void Cleanup()
        {
            saveLoadSystem.Release -= Cleanup;
            saveLoadSystem.SettingsSet -= PlayMenuMusic;
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
            StartMenuState startState = new StartMenuState(this, screens[START], darkerBackground, optionMenus[START], menuControllers[START - 2], 0.5f);
            SettingsMenuState settingsState = new SettingsMenuState(this, screens[SETTINGS], darkerBackground, optionMenus[SETTINGS], menuControllers[SETTINGS - 2], 0.5f);
            AudioMenuState audioState = new AudioMenuState(this, screens[AUDIO], darkerBackground, optionMenus[AUDIO], menuControllers[AUDIO - 2], 0.5f);
            VideoMenuState videoState = new VideoMenuState(this, screens[VIDEO], darkerBackground, optionMenus[VIDEO], menuControllers[VIDEO - 2], 0.5f);
            ControlsMenuState controlsState = new ControlsMenuState(this, screens[CONTROLS], darkerBackground, optionMenus[CONTROLS], menuControllers[CONTROLS - 2], 0.5f);

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

        private void PlayMenuMusic()
        {
            // Set the menu state
            MusicManager.Instance.SetState(menuState);

            // Ensure the menu music is playing
            MusicManager.Instance.Play();
        }

        /// <summary>
        /// Set the current state of the Main Menu Controller
        /// </summary>
        public void SetState(int state) => currentState = state;

        /// <summary>
        /// Set the Menu Controller
        /// </summary>
        public void SetMenuController(IMenuController menuController) => currentMenuController = menuController;

        /// <summary>
        /// Backtrack to the previous menu
        /// </summary>
        private void Backtrack(bool started)
        {
            // Exit case - the button is being lifted
            if (!started) return;

            // Exit case - there's no current menu controller
            if (currentMenuController == null) return;

            // Call the Back() function
            currentMenuController.Back();

            // Post the backtrack event
            backtrackEvent.Post(gameObject);
        }

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

            // Post the enter initial event
            enterInitialEvent.Post(gameObject);
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame() => Application.Quit();

        /// <summary>
        /// Check if the cursor is enabled
        /// </summary>
        public void Transmute(string currentControlScheme)
        {
            switch (currentControlScheme)
            {
                case "Keyboard and Mouse":
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                case "Xbox Controller":
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case "PlayStation":
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
            }
        }
    }
}

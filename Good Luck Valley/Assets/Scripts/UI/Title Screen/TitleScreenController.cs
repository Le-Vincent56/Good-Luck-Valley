using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI.TitleScreen.States;
using GoodLuckValley.UI.Menus;
using GoodLuckValley.Audio.Music;
using GoodLuckValley.Audio.Ambience;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.TitleScreen
{
    [Serializable]
    public struct Exclusions
    {
        public List<Graphic> Objects;
    }

    public class TitleScreenController : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playNavigate;
        [SerializeField] private AK.Wwise.Event playButtonEnter;
        [SerializeField] private AK.Wwise.Event playButtonExit;

        [Header("Events")]
        [SerializeField] private GameEvent onMainMenuBack;

        [Header("References")]
        [SerializeField] private MenuInputReader inputReader;
        [SerializeField] private TitleBackgroundFade backgroundFade;

        [Header("Fields")]
        [SerializeField] private int state;
        [SerializeField] private bool tryingToChangeState;
        [SerializeField] private bool changingState;
        [SerializeField] private float backTimer = 0f;
        [SerializeField] private float backBuffer = 0.3f;
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
            ControlsMenuState controlsState = new ControlsMenuState(this, stateMachine, true, exclusions[CONTROLS], screens[CONTROLS], cursors[CURSOR_SETTINGS_CONTROLS]);

            // Exclude certain elements
            startState.AddExcludeds(exclusions[START].Objects);

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

        private void Start()
        {
            inputReader.Enable();

            // Stop ambience
            AmbienceManager.Instance.StopAmbience();

            // Set menu states
            MusicManager.Instance.SetMenuStates();

            // Bind data
            SaveLoadSystem.Instance.BindSettings(true);
        }

        private void OnEnable()
        {
            inputReader.Start += InitializeMenu;
            inputReader.Escape += Back;
            inputReader.Navigate += NavigateUI;
            inputReader.Submit += SubmitUI;
        }

        private void OnDisable()
        {
            inputReader.Start -= InitializeMenu;
            inputReader.Escape -= Back;
            inputReader.Navigate -= NavigateUI;
            inputReader.Submit -= SubmitUI;
        }

        private void Update()
        {
            // Update the back timer
            if (backTimer > 0f)
                backTimer -= Time.deltaTime;

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
        public void SetState(int state)
        {
            // Compare states
            if (state > this.state)
                // If greater, play the enter sound
                playButtonEnter.Post(gameObject);
            else if(state < this.state)
                // If less, then play exit sound
                playButtonExit.Post(gameObject);

            // Set the state
            this.state = state;

            // Set the back timer to prevent escaping
            backTimer = backBuffer;
        }

        /// <summary>
        /// Handle back input
        /// </summary>
        /// <param name="started"></param>
        public void Back(bool started)
        {
            // Exit case - if the key has not been lifted yet
            if (started) return;

            // Exit case - if the back timer is not finished yet
            if (backTimer > 0f) return;

            // Set the back timer
            backTimer = backBuffer;

            // Raise the event
            onMainMenuBack.Raise(this, state);
        }

        /// <summary>
        /// Handle menu navigation with keys
        /// </summary>
        /// <param name="navigation"></param>
        private void NavigateUI(Vector2 navigation)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null)
            {
                // If nothing is selected, select the first selectable element
                Selectable firstSelectable = FindObjectOfType<Selectable>();
                if (firstSelectable != null)
                {
                    EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
                }
                return;
            }

            Selectable current = currentSelected.GetComponent<Selectable>();
            Selectable next = null;

            if (navigation.y > 0) next = current.FindSelectableOnUp();
            else if (navigation.y < 0) next = current.FindSelectableOnDown();
            else if (navigation.x < 0) next = current.FindSelectableOnLeft();
            else if (navigation.x > 0) next = current.FindSelectableOnRight();

            if (next != null)
            {
                playNavigate.Post(gameObject);
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
        }

        /// <summary>
        /// Handle submit actions
        /// </summary>
        /// <param name="started"></param>
        private void SubmitUI(bool started)
        {
            // Exit case - if the button hasn't been released yet
            if (started) return;

            // Get the currently selected object
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            // Exit case - if there is not a currently selected object
            if (currentSelected == null) return;

            // Try to get a selectable
            Selectable selectable = currentSelected.GetComponent<Selectable>();

            // Exit case - if no selectable is retrieved
            if (selectable == null) return;

            // Check if the selectable is a button
            if (selectable is Button selectableButton)
            {
                // Cast and invoke
                selectableButton.onClick.Invoke();
            }
            // Check if the selectable is a toggle
            else if (selectable is Toggle selectableToggle)
            {
                // Cast and toggle
                selectableToggle.isOn = !selectableToggle.isOn;
            }
            // Check if the selectable is a dropdown
            else if (selectable is Dropdown dropdown) 
            {
                // Show the dropdown
                dropdown.Show();
            }
        }
    }
}
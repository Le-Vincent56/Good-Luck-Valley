using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Player.Input;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class TitleScreenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MenuInputReader inputReader;
        [SerializeField] private MainMenuCursor mainCursor;
        [SerializeField] private StartMenuCursor startMenuCursor;

        [Header("Fields")]
        [SerializeField] private int state;
        [SerializeField] private List<GameObject> screens = new List<GameObject>();

        private StateMachine stateMachine;

        public int INIT { get => 0; }
        public int MAIN { get => 1; }
        public int START { get => 2; }
        public int SETTINGS { get => 1; }

        private void Awake()
        {
            // Set time scale to normal time
            Time.timeScale = 1.0f;

            // Set the state to the initialize screen
            state = INIT;

            // Get components
            mainCursor = GetComponentInChildren<MainMenuCursor>();
            startMenuCursor = GetComponentInChildren<StartMenuCursor>();

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Construct states
            InitialMenuState initialState = new InitialMenuState(this, stateMachine, true, screens[INIT]);
            MainMenuState mainState = new MainMenuState(this, stateMachine, true, screens[MAIN], mainCursor);
            StartMenuState startState = new StartMenuState(this, stateMachine, true, screens[START], startMenuCursor);

            // Set state transitions
            At(initialState, mainState, new FuncPredicate(() => state == MAIN));
            At(mainState, startState, new FuncPredicate(() => state == START));
            At(startState, mainState, new FuncPredicate(() => state == MAIN));

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
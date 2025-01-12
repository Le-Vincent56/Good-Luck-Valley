using GoodLuckValley.Input;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.UI.MainMenu.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GoodLuckValley.UI.MainMenu.OptionMenus;
using Sirenix.OdinInspector;

namespace GoodLuckValley.UI.MainMenu
{
    public class MainMenuController : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIInputReader inputReader;

        [Header("States")]
        [SerializeField] private int currentState;
        [SerializeField] private List<CanvasGroup> screens;
        [SerializeField] private List<IOptionMenu> optionMenus;
        private StateMachine stateMachine;

        public int OPEN => 0;
        public int INITIAL => 1;

        private void Awake()
        {
            // Get the screens
            screens = GetComponentsInChildren<CanvasGroup>().ToList();
            optionMenus = GetComponentsInChildren<IOptionMenu>().ToList();

            // Set the current state to 0
            currentState = 0;

            // Set up the State Machine
            SetupStateMachine();

            // Set the UI input reader
            inputReader.Set();
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
            OpenMenuState openState = new OpenMenuState(this, screens[OPEN], optionMenus[OPEN]);
            InitialMenuState initialState = new InitialMenuState(this, screens[INITIAL], optionMenus[INITIAL]);

            // Define state transitions
            stateMachine.At(openState, initialState, new FuncPredicate(() => currentState == INITIAL));

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

            // Set the initial state
            SetState(INITIAL);
        }
    }
}

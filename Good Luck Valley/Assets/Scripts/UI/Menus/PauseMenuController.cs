using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Input;
using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader gameInputReader;
        [SerializeField] private UIInputReader menuInputReader;
        private CanvasGroup[] canvasGroups;
        private IOptionMenu[] optionMenus;

        [Header("Variables")]
        [SerializeField] private bool paused;
        [SerializeField] private int state;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private StateMachine stateMachine;

        public bool Paused { get => paused; set => paused = value; }
        public int UNPAUSED => -1;
        public int PAUSED => 0;
        public int SETTINGS => 1;

        private void Awake()
        {
            // Get components
            canvasGroups = GetComponentsInChildren<CanvasGroup>();
            optionMenus = GetComponentsInChildren<IOptionMenu>();

            // Initialize the State Machine
            InitializeStateMachine();

            // Disable the menu input reader
            menuInputReader.Disable();
        }

        private void OnEnable()
        {
            gameInputReader.Pause += PauseGame;
            menuInputReader.Cancel += UnpauseGame;
        }

        private void OnDisable()
        {
            gameInputReader.Pause -= PauseGame;
            menuInputReader.Cancel -= UnpauseGame;
        }

        private void Update()
        {
            // Exit case - if not paused
            if (!paused) return;

            // Update the State Machine
            stateMachine.Update();
        }

        /// <summary>
        /// Initialize the State Machine
        /// </summary>
        private void InitializeStateMachine()
        {
            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Create states
            InitialPauseState pauseMenuState = new InitialPauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], fadeDuration);
        }

        /// <summary>
        /// Enable Game Input
        /// </summary>
        public void EnableGameInput()
        {
            // Toggle inputs
            gameInputReader.Enable();
            menuInputReader.Disable();
        }

        /// <summary>
        /// Enable Menu Input
        /// </summary>
        public void EnableMenuInput()
        {
            // Toggle inputs
            gameInputReader.Disable();
            menuInputReader.Enable();
        }

        private void PauseGame(bool started)
        {
            // Exit case - if the button is pressed but not lifted
            if (started) return;

            // Set the state to paused
            state = 0;

            // Set paused
            paused = true;
        }

        private void UnpauseGame(bool started)
        {
            // Exit case - if the button is pressed but not lifted
            if (started) return;

            state = -1;
        }
    }
}

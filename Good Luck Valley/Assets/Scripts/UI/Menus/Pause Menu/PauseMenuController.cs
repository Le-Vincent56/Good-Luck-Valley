using DG.Tweening;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.Events.Pause;
using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes;
using GoodLuckValley.UI.Input;
using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.Pause.States;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Pause
{
    public class PauseMenuController : MonoBehaviour, ITransmutableInputUI
    {
        [Header("References")]
        [SerializeField] private GameInputReader gameInputReader;
        [SerializeField] private UIInputReader menuInputReader;
        [SerializeField] private Image background;
        [SerializeField] private List<CanvasGroup> canvasGroups;
        [SerializeField] private List<IOptionMenu> optionMenus;
        [SerializeField] private List<IMenuController> menuControllers;
        private IMenuController currentMenuController;
        private ControlSchemeDetector inputDetector;
        private SaveLoadSystem saveLoadSystem;
        private SceneLoader sceneLoader;

        [Header("Variables")]
        [SerializeField] private bool paused;
        [SerializeField] private bool journalUnlocked;
        [SerializeField] private int state;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.State menuState;
        [SerializeField] private AK.Wwise.Event backtrackEvent;

        private StateMachine stateMachine;

        private EventBinding<ShowPauseMenuFromJournal> onShowPauseMenuFromJournal;
        private EventBinding<UpdateJournalUnlock> onUpdateJournalUnlock;

        public bool Paused { get => paused; set => paused = value; }
        public int UNPAUSED => -1;
        public int PAUSED => 0;
        public int SETTINGS => 1;
        public int AUDIO => 2;
        public int VIDEO => 3;
        public int CONTROLS => 4;
        public int JOURNAL => 5;

        private void Awake()
        {
            canvasGroups = new List<CanvasGroup>();
            optionMenus = new List<IOptionMenu>();
            menuControllers = new List<IMenuController>();

            // Get components
            GetComponentsInChildren(canvasGroups);
            GetComponentsInChildren(optionMenus);
            GetComponentsInChildren(menuControllers);

            // Initialize the State Machine
            InitializeStateMachine();

            // Disable the menu input reader
            menuInputReader.Disable();

            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
            inputDetector = ServiceLocator.Global.Get<ControlSchemeDetector>();
        }

        private void OnEnable()
        {
            gameInputReader.Pause += PauseGame;
            menuInputReader.Cancel += Backtrack;

            onShowPauseMenuFromJournal = new EventBinding<ShowPauseMenuFromJournal>(ReturnToPauseFromJournal);
            EventBus<ShowPauseMenuFromJournal>.Register(onShowPauseMenuFromJournal);

            onUpdateJournalUnlock = new EventBinding<UpdateJournalUnlock>(UpdateJournalUnlock);
            EventBus<UpdateJournalUnlock>.Register(onUpdateJournalUnlock);

            inputDetector.Register(this);
        }

        private void OnDisable()
        {
            gameInputReader.Pause -= PauseGame;
            menuInputReader.Cancel -= Backtrack;

            EventBus<ShowPauseMenuFromJournal>.Deregister(onShowPauseMenuFromJournal);
            EventBus<UpdateJournalUnlock>.Deregister(onUpdateJournalUnlock);

            inputDetector.Deregister(this);
        }

        private void Update()
        {
            // Exit case - if not paused
            if (!paused) return;

            // Update the State Machine
            stateMachine.Update();
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        /// <summary>
        /// Initialize the State Machine
        /// </summary>
        private void InitializeStateMachine()
        {
            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Create states
            ResumePauseState resumeState = new ResumePauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], fadeDuration);
            InitialPauseState pauseState = new InitialPauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], menuControllers[PAUSED], fadeDuration);
            JournalPauseState journalState = new JournalPauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], fadeDuration);
            SettingsPauseState settingsState = new SettingsPauseState(this, canvasGroups[SETTINGS], optionMenus[SETTINGS], menuControllers[SETTINGS], fadeDuration);
            AudioPauseState audioState = new AudioPauseState(this, canvasGroups[AUDIO], optionMenus[AUDIO], menuControllers[AUDIO], fadeDuration);
            VideoPauseState videoState = new VideoPauseState(this, canvasGroups[VIDEO], optionMenus[VIDEO], menuControllers[VIDEO], fadeDuration);
            ControlsPauseState controlsState = new ControlsPauseState(this, canvasGroups[CONTROLS], optionMenus[CONTROLS], menuControllers[CONTROLS], fadeDuration);

            // Define state transitions
            stateMachine.At(pauseState, resumeState, new FuncPredicate(() => state == UNPAUSED));
            stateMachine.At(pauseState, journalState, new FuncPredicate(() => state == JOURNAL));
            stateMachine.At(pauseState, settingsState, new FuncPredicate(() => state == SETTINGS));

            stateMachine.At(resumeState, pauseState, new FuncPredicate(() => state == PAUSED));

            stateMachine.At(journalState, pauseState, new FuncPredicate(() => state == PAUSED));

            stateMachine.At(settingsState, pauseState, new FuncPredicate(() => state == PAUSED));
            stateMachine.At(settingsState, audioState, new FuncPredicate(() => state == AUDIO));
            stateMachine.At(settingsState, videoState, new FuncPredicate(() => state == VIDEO));
            stateMachine.At(settingsState, controlsState, new FuncPredicate(() => state == CONTROLS));

            stateMachine.At(audioState, settingsState, new FuncPredicate(() => state == SETTINGS));

            stateMachine.At(videoState, settingsState, new FuncPredicate(() => state == SETTINGS));

            stateMachine.At(controlsState, settingsState, new FuncPredicate(() => state == SETTINGS));

            // Set the initial state
            stateMachine.SetState(resumeState);
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

        /// <summary>
        /// Show the Pause Menu background
        /// </summary>
        public void ShowBackground(Action onShown = null)
        {
            // Disable the input readers
            gameInputReader.Disable();
            menuInputReader.Disable();

            // Fade and re-enable the menu input reader
            Fade(0.8f, fadeDuration, () =>
            {
                // Enable the menu input reader
                menuInputReader.Enable();

                // Exit case - there is no completion action
                if (onShown == null) return;

                // Call the completion action
                onShown();
            });
        }

        public void HideBackground(TweenCallback onComplete = null)
        {
            // Fade out
            Fade(0f, fadeDuration, () =>
            {
                // Exit case - there is no completion action
                if (onComplete == null) return;

                // Call the completion action
                onComplete();
            });
        }

        /// <summary>
        /// Hide the Pause Menu background
        /// </summary>
        public void HideBackgroundExit(TweenCallback onComplete = null)
        {
            // Disable the input readers
            gameInputReader.Disable();
            menuInputReader.Disable();

            Fade(0f, fadeDuration, () =>
            {
                // Enable the input reader
                gameInputReader.Enable();

                // Exit case - there is no completion action
                if (onComplete == null) return;

                // Call the completion action
                onComplete();
            });
        }

        /// <summary>
        /// Input callback for pausing the game
        /// </summary>
        private void PauseGame(bool started)
        {
            // Exit case - if the button is pressed but not lifted
            if (started) return;

            // Exit case - if the game is loading
            if(sceneLoader.IsLoading) return;

            // Set the state to paused
            state = 0;

            // Set paused
            paused = true;
        }

        /// <summary>
        /// Handle event for backtracking through menus
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
        /// Set the state of the Pause Menu Controller
        /// </summary>
        public void SetState(int state) => this.state = state;

        /// <summary>
        /// Set the current Menu Controller
        /// </summary>
        public void SetMenuController(IMenuController menuController) => currentMenuController = menuController;

        /// <summary>
        /// Event callback to update the status of the Journal unlock
        /// </summary>
        private void UpdateJournalUnlock(UpdateJournalUnlock eventData) => journalUnlocked = eventData.Unlocked;

        /// <summary>
        /// Open the Journal from the Pause Menu
        /// </summary>
        public void OpenJournal()
        {
            // Exit case - if the Journal is not unlocked
            if (!journalUnlocked) return;

            // Set the Journal State
            SetState(JOURNAL);
        }

        /// <summary>
        /// Event callback to return to the pause menu from the Journal
        /// </summary>
        private void ReturnToPauseFromJournal() => SetState(PAUSED);

        /// <summary>
        /// Save the game
        /// </summary>
        public void Save() => saveLoadSystem.SaveGame();

        /// <summary>
        /// Handle Fade Tweening for the Pause Menu
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the fade tween
            fadeTween = background.DOFade(endValue, duration);

            // Ignore time scale
            fadeTween.SetUpdate(true);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Return to the Main Menu
        /// </summary>
        public void ReturnToMain()
        {
            // Set the menu state
            MusicManager.Instance.SetState(menuState);

            // Stop ambience
            AmbienceManager.Instance.StopAmbience();

            // Nullify any forced move directions
            sceneLoader.ForcedMoveDirection = 0;

            // Load the Scene Group
            sceneLoader.ChangeSceneGroupSystem(0);
        }

        public void Transmute(string currentControlScheme)
        {
            // Exit case - not paused
            if (!paused) return;

            // Check if the cursor should be shown
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

using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.Events.Pause;
using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes;
using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.Pause.States;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Pause
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader gameInputReader;
        [SerializeField] private UIInputReader menuInputReader;
        [SerializeField] private Image background;
        [SerializeField] private CanvasGroup[] canvasGroups;
        [SerializeField] private IOptionMenu[] optionMenus;
        [SerializeField] private IMenuController[] menuControllers;
        private IMenuController currentMenuController;

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
            // Get components
            canvasGroups = GetComponentsInChildren<CanvasGroup>();
            optionMenus = GetComponentsInChildren<IOptionMenu>();
            menuControllers = GetComponentsInChildren<IMenuController>();

            // Initialize the State Machine
            InitializeStateMachine();

            // Disable the menu input reader
            menuInputReader.Disable();
        }

        private void OnEnable()
        {
            gameInputReader.Pause += PauseGame;
            menuInputReader.Cancel += Backtrack;

            onShowPauseMenuFromJournal = new EventBinding<ShowPauseMenuFromJournal>(ReturnToPauseFromJournal);
            EventBus<ShowPauseMenuFromJournal>.Register(onShowPauseMenuFromJournal);

            onUpdateJournalUnlock = new EventBinding<UpdateJournalUnlock>(UpdateJournalUnlock);
            EventBus<UpdateJournalUnlock>.Register(onUpdateJournalUnlock);
        }

        private void OnDisable()
        {
            gameInputReader.Pause -= PauseGame;
            menuInputReader.Cancel -= Backtrack;

            EventBus<ShowPauseMenuFromJournal>.Deregister(onShowPauseMenuFromJournal);
            EventBus<UpdateJournalUnlock>.Deregister(onUpdateJournalUnlock);
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
        public void ShowBackground()
        {
            // Disable the input readers
            gameInputReader.Disable();
            menuInputReader.Disable();

            // Fade and re-enable the menu input reader
            Fade(0.8f, fadeDuration, () => menuInputReader.Enable());
        }

        /// <summary>
        /// Hide the Pause Menu background
        /// </summary>
        public void HideBackground(TweenCallback onComplete = null)
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
            if(SceneLoader.Instance.IsLoading) return;

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
        public void Save() => SaveLoadSystem.Instance.SaveGame();

        /// <summary>
        /// Set the state to unpaused
        /// </summary>
        public void Back() => state = -1;

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

            // Load the Scene Group
            SceneLoader.Instance.ChangeSceneGroupSystem(0);
        }
    }
}

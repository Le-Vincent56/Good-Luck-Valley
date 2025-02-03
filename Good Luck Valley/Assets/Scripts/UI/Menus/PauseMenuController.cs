using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes;
using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.States;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader gameInputReader;
        [SerializeField] private UIInputReader menuInputReader;
        [SerializeField] private Image background;
        [SerializeField] private CanvasGroup[] canvasGroups;
        [SerializeField] private IOptionMenu[] optionMenus;

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
        public int AUDIO => 2;
        public int VIDEO => 3;
        public int CONTROLS => 4;

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
            ResumePauseState resumeState = new ResumePauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], fadeDuration);
            InitialPauseState pauseState = new InitialPauseState(this, canvasGroups[PAUSED], optionMenus[PAUSED], fadeDuration);
            SettingsPauseState settingsState = new SettingsPauseState(this, canvasGroups[SETTINGS], optionMenus[SETTINGS], fadeDuration);
            AudioPauseState audioState = new AudioPauseState(this, canvasGroups[AUDIO], optionMenus[AUDIO], fadeDuration);
            VideoPauseState videoState = new VideoPauseState(this, canvasGroups[VIDEO], optionMenus[VIDEO], fadeDuration);
            ControlsPauseState controlsState = new ControlsPauseState(this, canvasGroups[CONTROLS], optionMenus[CONTROLS], fadeDuration);

            // Define state transitions
            stateMachine.At(pauseState, resumeState, new FuncPredicate(() => state == UNPAUSED));
            stateMachine.At(pauseState, settingsState, new FuncPredicate(() => state == SETTINGS));

            stateMachine.At(resumeState, pauseState, new FuncPredicate(() => state == PAUSED));

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

            // Fade in the background
            Fade(0.8f, fadeDuration);
        }

        /// <summary>
        /// Input callback for unpausing the game
        /// </summary>
        private void UnpauseGame(bool started)
        {
            // Exit case - if the button is pressed but not lifted
            if (started) return;

            // Set unpaused
            state = -1;

            // Fade out the background
            Fade(0f, fadeDuration);
        }

        /// <summary>
        /// Set the state of the Pause Menu Controller
        /// </summary>
        public void SetState(int state) => this.state = state;

        /// <summary>
        /// Save the game
        /// </summary>
        public void Save() => SaveLoadSystem.Instance.SaveGame();

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

        public void ReturnToMain()
        {
            // Load the Scene Group
            SceneLoader.Instance.ChangeSceneGroupSystem(0);
        }
    }
}

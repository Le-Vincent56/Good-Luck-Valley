using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Timers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GoodLuckValley.Events.Player;
using GoodLuckValley.Events.Scenes;
using GoodLuckValley.Persistence;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using GoodLuckValley.Events.Mushroom;

namespace GoodLuckValley.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        private SaveLoadSystem saveLoadSystem;

        [Header("Scenes")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SceneGroupData sceneGroupData;
        private bool isLoading;
        public SceneGroupManager Manager;

        private SceneGate toGate;
        private int forcedMoveDirection = 0;
        private CountdownTimer releaseMovementTimer;

        public SceneGroup[] SceneGroups { get => sceneGroupData.SceneGroups; }

        public UnityAction Release = delegate { };

        public bool IsLoading { get => isLoading; }
        public int ForcedMoveDirection { get => forcedMoveDirection; set=> forcedMoveDirection = value; }

        private void Awake()
        {
            Manager = new SceneGroupManager();

            // Create the Countdown Timer
            releaseMovementTimer = new CountdownTimer(1.5f);

            releaseMovementTimer.OnTimerStop += () =>
            {
                // Stop forced player movement
                EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
                {
                    ForcedMove = false,
                    ForcedMoveDirection = 0
                });

                // Allow the player to input the mushroom
                EventBus<SetMushroomInput>.Raise(new SetMushroomInput()
                {
                    CanInputMushroom = true
                });
            };

            // Register this as a service
            ServiceLocator.Global.Register(this);

            // get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
        }

        private async void Start()
        {
            await LoadSceneGroup(sceneGroupData.InitialScene);
        }

        private void OnEnable()
        {
            saveLoadSystem.Release += Cleanup;
            saveLoadSystem.DataBinded += SetPlayerPosition;
        }

        private void OnDestroy()
        {
            // Dispose of the Timer
            releaseMovementTimer?.Dispose();

            // Clean up events
            Release.Invoke();
        }

        /// <summary>
        /// Cleanup by unsubscribing from events from the Save Load System
        /// </summary>
        private void Cleanup()
        {
            saveLoadSystem.Release -= Cleanup;
            saveLoadSystem.DataBinded -= SetPlayerPosition;
        }

        /// <summary>
        /// Event for when a Scene Group is loaded
        /// </summary>
        private void SetPlayerPosition(int index)
        {
            // Set the time scale
            Time.timeScale = 1f;

            if (forcedMoveDirection != 0)
            {
                // Place the player
                EventBus<PlacePlayer>.Raise(new PlacePlayer()
                {
                    Position = sceneGroupData.GetActiveScene(index).GetGate(toGate)
                });
            }

            HandleLoading(false, forcedMoveDirection);
        }

        /// <summary>
        /// Load a Scene Group
        /// </summary>
        public async Task LoadSceneGroup(int index)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Start loading
            HandleLoading(true);

            await Manager.LoadScenes(index, sceneGroupData.SceneGroups[index], progress);
        }

        /// <summary>
        /// Change from one Scene Group to another during gameplay levels
        /// </summary>
        public void ChangeSceneGroupLevel(int index, SceneGate toGate, int lastMovingDirection = 0)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Set the forced movement direction
            forcedMoveDirection = lastMovingDirection;

            // Start loading
            HandleLoading(true);

            this.toGate = toGate;

            // Fade in the loading image
            EventBus<FadeScene>.Raise(new FadeScene()
            {
                FadeIn = false,
                EaseType = Ease.InQuad,
                OnComplete = async () => await Manager.LoadScenes(index, sceneGroupData.SceneGroups[index], progress, true)
            });

            // Don't allow the player to input the mushroom
            EventBus<SetMushroomInput>.Raise(new SetMushroomInput()
            {
                CanInputMushroom = false
            });
        }

        /// <summary>
        /// Change from one Scene Group to another using a system
        /// </summary>
        public void ChangeSceneGroupSystem(int index)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            forcedMoveDirection = 0;

            // Start loading
            HandleLoading(true);

            // Fade in the loading image
            EventBus<FadeScene>.Raise(new FadeScene()
            {
                FadeIn = false,
                EaseType = Ease.InQuad,
                OnComplete = async () => await Manager.LoadScenes(index, sceneGroupData.SceneGroups[index], progress, true)
            });
        }

        /// <summary>
        /// Handle loading for the Scene Loader
        /// </summary>
        private void HandleLoading(bool isLoading, int forcedMoveDirection = 0)
        {
            // Set isLoading
            this.isLoading = isLoading;

            // Check if loading
            if (!isLoading)
            {
                // Fade in the scene
                EventBus<FadeScene>.Raise(new FadeScene()
                {
                    FadeIn = true,
                    EaseType = Ease.OutQuad,
                    OnComplete = null
                });

                // Exit case - if not forcing a movement direction
                if (forcedMoveDirection == 0)
                {
                    // Allow the player to input the mushroom
                    EventBus<SetMushroomInput>.Raise(new SetMushroomInput()
                    {
                        CanInputMushroom = true
                    });
                    return;
                }

                // Force player movement
                EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
                {
                    ForcedMove = true,
                    ForcedMoveDirection = forcedMoveDirection
                });

                // Don't allow the player to input the mushroom
                EventBus<SetMushroomInput>.Raise(new SetMushroomInput()
                {
                    CanInputMushroom = false
                });

                // Start the release movement Timer
                releaseMovementTimer.Start();
            }
        }
    }
}

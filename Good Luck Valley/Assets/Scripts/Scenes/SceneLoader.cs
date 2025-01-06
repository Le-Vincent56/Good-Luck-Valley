using DG.Tweening;
using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Architecture.Singletons;
using GoodLuckValley.Timers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GoodLuckValley.Scenes
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [Header("Scenes")]
        [SerializeField] private SceneGroupData sceneGroupData;
        private bool isLoading;
        public SceneGroupManager Manager;

        private int forcedMoveDirection = 0;
        private CountdownTimer releaseMovementTimer;

        public SceneGroup[] SceneGroups { get => sceneGroupData.SceneGroups; }

        public UnityAction Cleanup = delegate { };

        protected override void Awake()
        {
            // Set up the Singleton
            base.Awake();

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
            };
        }

        private async void Start()
        {
            await LoadSceneGroup(sceneGroupData.InitialScene);
        }

        private void OnEnable()
        {
            Manager.OnSceneGroupLoaded += (int index) => 
            { 
                HandleLoading(false, forcedMoveDirection); 
                /*EventBusUtils.Debug();*/  
            };
        }

        private void OnDisable()
        {
            Manager.OnSceneGroupLoaded -= (int indx) => HandleLoading(false, forcedMoveDirection);
        }

        private void OnDestroy()
        {
            // Dispose of the Timer
            releaseMovementTimer?.Dispose();

            // Clean up events
            Cleanup.Invoke();
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
        /// Change from one Scene Group to another
        /// </summary>
        public void ChangeSceneGroup(int index, int lastMovingDirection = 0)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Set the forced movement direction
            forcedMoveDirection = lastMovingDirection;

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
                EventBus<FadeScene>.Raise(new FadeScene()
                {
                    FadeIn = true,
                    EaseType = Ease.OutQuad,
                    OnComplete = null
                });

                // Exit case - if not forcing a movement direction
                if (forcedMoveDirection == 0) return;

                // Force player movement
                EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
                {
                    ForcedMove = true,
                    ForcedMoveDirection = forcedMoveDirection
                });

                // Start the release movement Timer
                releaseMovementTimer.Start();
            }
        }
    }
}

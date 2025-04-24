using Cysharp.Threading.Tasks;
using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Timers;
using UnityEngine;
using GoodLuckValley.Events.Player;
using GoodLuckValley.Events.Scenes;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using GoodLuckValley.Events.Mushroom;
using System.Collections.Generic;
using System;

namespace GoodLuckValley.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SceneGroupData sceneGroupData;
        private bool isLoading;
        public SceneGroupManager Manager;

        [Header("Fields")]
        private SceneGate toGate;
        [SerializeField] private bool loadingFromGate = false;
        [SerializeField] private int forcedMoveDirection = 0;
        [SerializeField] private bool showLoadingSymbol = true;
        private CountdownTimer releaseMovementTimer;

        private List<(Func<UniTask> Task, int Priority)> prePlaceActions;
        private List<(Func<UniTask> Task, int Priority)> postPlaceActions;

        public SceneGroup[] SceneGroups { get => sceneGroupData.SceneGroups; }

        public Action QueryTasks = delegate { };

        public bool IsLoading { get => isLoading; }
        public bool LoadingFromGate { get => loadingFromGate; set => loadingFromGate = value; }
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

            // Create the task lists
            prePlaceActions = new List<(Func<UniTask> Task, int Priority)>();
            postPlaceActions = new List<(Func<UniTask> Task, int Priority)>();

            // Register this as a service
            ServiceLocator.Global.Register(this);
        }

        private void OnEnable()
        {
            Manager.OnSceneGroupLoaded += QueryAndResolveTasks;
        }

        private void OnDisable()
        {
            Manager.OnSceneGroupLoaded -= QueryAndResolveTasks;
        }

        private async void Start()
        {
            await LoadSceneGroup(sceneGroupData.InitialScene);
        }

        private void OnDestroy()
        {
            // Dispose of the Timer
            releaseMovementTimer?.Dispose();
        }

        /// <summary>
        /// Query and resolve loading tasks
        /// </summary>
        private async void QueryAndResolveTasks(int index)
        {
            // Clear the current task queues
            prePlaceActions.Clear();
            postPlaceActions.Clear();

            // Add the tasks to the queue
            QueryTasks.Invoke();

            // Sort the queues by priority
            prePlaceActions.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            postPlaceActions.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            // Process the tasks
            await ProcessTasks();

            // Start the task queue
            async UniTask ProcessTasks()
            {
                // Iterate through the task queue
                for(int i = 0; i < prePlaceActions.Count; i++)
                {
                    // Get the task
                    UniTask task = prePlaceActions[i].Task.Invoke();

                    // Execute the task
                    await task;
                }

                // Set the player position
                SetPlayerPosition(index);

                // Iterate through the task queue
                for (int i = 0; i < postPlaceActions.Count; i++)
                {
                    // Get the task
                    UniTask task = postPlaceActions[i].Task.Invoke();

                    // Execute the task
                    await task;
                }

                HandleLoading(false, showLoadingSymbol, forcedMoveDirection);
            }
        }

        /// <summary>
        /// Load a Scene Group
        /// </summary>
        public async UniTask LoadSceneGroup(int index)
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
        public void ChangeSceneGroupLevel(int index, SceneGate toGate, bool showLoadingSymbol = true, int lastMovingDirection = 0)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Set the forced movement direction
            forcedMoveDirection = lastMovingDirection;
            this.showLoadingSymbol = showLoadingSymbol;

            // Start loading
            HandleLoading(true, showLoadingSymbol);

            this.toGate = toGate;

            // Fade in the loading image
            EventBus<FadeScene>.Raise(new FadeScene()
            {
                FadeIn = false,
                ShowLoadingSymbol = showLoadingSymbol,
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
        public void ChangeSceneGroupSystem(int index, bool showLoadingSymbol = true)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            forcedMoveDirection = 0;
            this.showLoadingSymbol = showLoadingSymbol;

            // Start loading
            HandleLoading(true, showLoadingSymbol);

            // Fade in the loading image
            EventBus<FadeScene>.Raise(new FadeScene()
            {
                FadeIn = false,
                ShowLoadingSymbol = showLoadingSymbol,
                EaseType = Ease.InQuad,
                OnComplete = async () => await Manager.LoadScenes(index, sceneGroupData.SceneGroups[index], progress, true)
            });
        }

        /// <summary>
        /// Handle loading for the Scene Loader
        /// </summary>
        private void HandleLoading(bool isLoading, bool showLoadingSymbol = true, int forcedMoveDirection = 0)
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
                    ShowLoadingSymbol = showLoadingSymbol,
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

        /// <summary>
        /// Set the player position on load
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

            loadingFromGate = false;
        }

        /// <summary>
        /// Register a task to be processed after loading the scene group and before the player is placed
        /// </summary>
        public void RegisterPreTask(Func<UniTask> task, int priority) => prePlaceActions.Add((task, priority));

        /// <summary>
        /// Register a task to be processed before loading the scene group and after the player is placed
        /// </summary>
        public void RegisterPostTask(Func<UniTask> task, int priority) => postPlaceActions.Add((task, priority));
    }
}

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
using ZLinq;

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

        private List<(UniTask Task, int Priority)> taskQueue;

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

            // Create the task list
            taskQueue = new List<(UniTask task, int Priority)>();

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
            // Clear the current queue
            taskQueue.Clear();

            // Add the tasks to the queue
            QueryTasks.Invoke();

            // Sort the queue by priority
            taskQueue.AsValueEnumerable().OrderBy(task => task.Priority);

            // Process the tasks
            await ProcessTasks();

            // Start the task queue
            async UniTask ProcessTasks()
            {
                // Iterate through the task queue
                for(int i = 0; i < taskQueue.Count; i++)
                {
                    // Get the task
                    UniTask task = taskQueue[i].Task;

                    // Execute the task
                    await task;
                }

                // Set the player position
                SetPlayerPosition(index);
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

            loadingFromGate = false;

            HandleLoading(false, showLoadingSymbol, forcedMoveDirection);
        }

        /// <summary>
        /// Register a task to be processed after loading the scene group
        /// </summary>
        public void RegisterTask(UniTask task, int priority) => taskQueue.Add((task, priority));
    }
}

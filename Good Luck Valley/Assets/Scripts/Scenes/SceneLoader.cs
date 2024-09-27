using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Singletons;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes.Data;
using GoodLuckValley.World.AreaTriggers;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.SceneManagement
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onPrepareLevel;
        [SerializeField] private GameEvent onPreEntry;
        [SerializeField] private GameEvent onPrepareSettings;
        [SerializeField] private GameEvent onFadeIn;
        [SerializeField] private GameEvent onFadeOut;
        [SerializeField] private GameEvent onTransitionBegin;
        [SerializeField] private GameEvent onTransitionEnd;

        [Header("References")]
        [SerializeField] private LevelPositionData levelData;

        [Header("Fields")]
        [SerializeField] private bool fromMainMenu;
        [SerializeField] private float transitionTime = 1f;
        [SerializeField] private bool isLoading;
        [SerializeField] private (string name, TransitionType type, int loadIndex) sceneToLoad;
        [SerializeField] private int transitionDirection;
        [SerializeField] private string previousScene;

        public bool IsLoading { get { return isLoading; } }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Check if transitioning into a scene
            if (scene.name == sceneToLoad.name && isLoading)
            {
                // Bind data
                if (SaveLoadSystem.Instance.selectedData != null)
                    SaveLoadSystem.Instance.BindData(true);

                if (!fromMainMenu)
                {
                    Vector3 playerPos;
                    LevelDataContainer retrievedLevelData = levelData.GetLevelData(sceneToLoad.name);

                    if(retrievedLevelData != null)
                    {
                        if (sceneToLoad.type == TransitionType.Entrance)
                        {
                            playerPos = retrievedLevelData.Entrances[sceneToLoad.loadIndex];
                        }
                        else
                        {
                            playerPos = retrievedLevelData.Exits[sceneToLoad.loadIndex];
                        }

                        (Vector2 pos, int dir) dataToSend;
                        dataToSend.pos = playerPos;
                        dataToSend.dir = transitionDirection;

                        // Prepare the level
                        // Calls to:
                        //  - Player.PreparePlayerPosition();
                        onPrepareLevel.Raise(this, dataToSend);
                    }
                }

                // Activate pre-entry effects
                // Calls to:
                //  - CameraFade.Blackout();
                onPreEntry.Raise(this, null);

                // Prepare settings
                // Calls to:
                //  - GameAudioSettingsController.Init();
                //  - GameVideoSettingsController.Init();
                //  - GameControlsSettingsController.Init();
                onPrepareSettings.Raise(this, null);

                // Finalize the level transition
                FinalizeLevel();
            }
        }

        private void Start()
        {
            // Start fading in
            // Calls to:
            // - CameraFade.PlayFadeIn();
            onFadeIn.Raise(this, true);
        }

        /// <summary>
        /// Finalize the level transition
        /// </summary>
        public void FinalizeLevel()
        {
            // Start fading in
            // Calls to:
            // - CameraFade.PlayFadeIn();
            onFadeIn.Raise(this, true);

            if (SaveLoadSystem.Instance.settingsData != null)
                SaveLoadSystem.Instance.BindSettings(true);
        }

        /// <summary>
        /// Set the scene to load
        /// </summary>
        /// <param name="sceneToLoad"></param>
        /// <param name="transitionType"></param>
        /// <param name="moveDirection"></param>
        /// <param name="loadIndex"></param>
        public void SetSceneToLoad(string sceneToLoad, TransitionType transitionType, int moveDirection, int loadIndex)
        {
            // Set the name of the scene
            this.sceneToLoad.name = sceneToLoad;

            // Check what kind of Transition Type it is
            if (transitionType == TransitionType.Entrance)
                this.sceneToLoad.type = TransitionType.Exit;
            else
                this.sceneToLoad.type = TransitionType.Entrance;

            // Set the load index
            this.sceneToLoad.loadIndex = loadIndex;

            // Set the transition direction
            transitionDirection = moveDirection;
        }

        /// <summary>
        /// Begin the scene transition
        /// </summary>
        public async void BeginTransition()
        {
            // Set previous scene
            previousScene = SceneManager.GetActiveScene().name;

            // Set loading to true
            isLoading = true;

            // Notify that it's not from the main menu
            fromMainMenu = false;

            // Begin any transition effects
            // Calls to:
            // - PlayerController.BeginPlayerTransition();
            onTransitionBegin.Raise(this, transitionDirection);

            // Wait for the transition time
            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            // Start fade out
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onFadeOut.Raise(this, null);
        }

        /// <summary>
        /// Have a fade-out only transition
        /// </summary>
        public void TransitionFadeOutOnly()
        {
            // Start fade out
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onFadeOut.Raise(this, null);
        }

        /// <summary>
        /// Change the scene
        /// </summary>
        public void ChangeScene() => SceneManager.LoadScene(sceneToLoad.name, LoadSceneMode.Single);

        /// <summary>
        /// End the scene transition
        /// </summary>
        public async void EndTransition()
        {
            // Wait for the transition time
            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            // End any transition effects
            // Calls to:
            //  - PlayerController.EndPlayerTransition();
            onTransitionEnd.Raise(this, null);

            // Set loading to false
            isLoading = false;
        }

        /// <summary>
        /// Scene transition specifically for entering the game from the main menu
        /// </summary>
        /// <param name="sceneName"></param>
        public void EnterGame(string sceneName)
        {
            sceneToLoad.name = sceneName;
            fromMainMenu = true;

            // Set loading
            isLoading = true;

            TransitionFadeOutOnly();
        }

        /// <summary>
        /// Scene transition specifically for going to the main menu
        /// </summary>
        public void LoadMainMenu()
        {
            // Set the main menu as the scene to load
            sceneToLoad.name = "Main Menu";
            fromMainMenu = true;

            // Set loading
            isLoading = true;

            TransitionFadeOutOnly();
        }
    }
}
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
                    // Set player position
                    Vector3 playerPos;
                    if (sceneToLoad.type == TransitionType.Entrance)
                    {
                        playerPos = levelData.GetLevelData(sceneToLoad.name).Entrances[sceneToLoad.loadIndex];
                    }
                    else
                    {
                        playerPos = levelData.GetLevelData(sceneToLoad.name).Exits[sceneToLoad.loadIndex];
                    }

                    (Vector2 pos, int dir) dataToSend;
                    dataToSend.pos = playerPos;
                    dataToSend.dir = transitionDirection;

                    // Prepare the level
                    // Calls to:
                    //  - Player.PreparePlayerPosition();
                    onPrepareLevel.Raise(this, dataToSend);
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

        public void FinalizeLevel()
        {
            // Start fading in
            // Calls to:
            // - CameraFade.PlayFadeIn();
            onFadeIn.Raise(this, true);

            if (SaveLoadSystem.Instance.settingsData != null)
                SaveLoadSystem.Instance.BindSettings(true);

            //StartTransitionTimer();
        }

        private async void StartTransitionTimer()
        {
            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            isLoading = false;
        }

        public void SetSceneToLoad(string sceneToLoad, TransitionType transitionType, int moveDirection, int loadIndex)
        {
            this.sceneToLoad.name = sceneToLoad;

            if (transitionType == TransitionType.Entrance)
                this.sceneToLoad.type = TransitionType.Exit;
            else
                this.sceneToLoad.type = TransitionType.Entrance;

            this.sceneToLoad.loadIndex = loadIndex;

            transitionDirection = moveDirection;
        }

        public async void BeginTransition()
        {
            // Set previous scene
            previousScene = SceneManager.GetActiveScene().name;

            isLoading = true;

            fromMainMenu = false;

            // Begin any transition effects
            // Calls to:
            // - PlayerController.BeginPlayerTransition();
            onTransitionBegin.Raise(this, transitionDirection);

            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            // Start fade out
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onFadeOut.Raise(this, null);
        }

        public void TransitionFadeOutOnly()
        {
            // Start fade out
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onFadeOut.Raise(this, null);
        }

        public void ChangeScene() => SceneManager.LoadScene(sceneToLoad.name, LoadSceneMode.Single);

        public async void EndTransition()
        {
            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            // End any transition effects
            // Calls to:
            //  - PlayerController.EndPlayerTransition();
            onTransitionEnd.Raise(this, null);

            // Set loading to false
            isLoading = false;
        }

        public void EnterGame(string sceneName)
        {
            sceneToLoad.name = sceneName;
            fromMainMenu = true;

            // Set loading
            isLoading = true;

            TransitionFadeOutOnly();
        }

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
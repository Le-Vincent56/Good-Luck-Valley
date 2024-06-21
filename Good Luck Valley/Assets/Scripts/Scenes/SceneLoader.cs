using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Singletons;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes.Data;
using GoodLuckValley.World.AreaTriggers;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.SceneManagement
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onPrepareLevel;
        [SerializeField] private GameEvent onFadeIn;
        [SerializeField] private GameEvent onFadeOut;
        [SerializeField] private GameEvent onTransitionBegin;
        [SerializeField] private GameEvent onTransitionEnd;

        [Header("References")]
        [SerializeField] private LevelPositionData levelData;

        [Header("Fields")]
        [SerializeField] private float transitionTime = 1f;
        [SerializeField] private bool isLoading;
        [SerializeField] private (string name, TransitionType type) sceneToLoad;
        [SerializeField] private int transitionDirection;
        [SerializeField] private string previousScene;

        public bool IsLoading { get { return isLoading; } }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Check if transitioning into a scene
            if(scene.name == sceneToLoad.name && isLoading)
            {
                // Set player position
                Vector3 playerPos;
                if (sceneToLoad.type == TransitionType.Entrance)
                {
                    playerPos = levelData.GetLevelData(sceneToLoad.name).Enter;
                }
                else
                {
                    playerPos = levelData.GetLevelData(sceneToLoad.name).Exit;
                }

                (Vector2 pos, int dir) dataToSend;
                dataToSend.pos = playerPos;
                dataToSend.dir = transitionDirection;

                // Prepare the level
                onPrepareLevel.Raise(this, dataToSend);

                // Start fading in
                // Calls to:
                // - CameraFade.PlayFadeIn();
                onFadeIn.Raise(this, true);

                SaveLoadSystem.Instance.SaveGame();
            }
        }

        public void SetSceneToLoad(string sceneToLoad, TransitionType transitionType, int moveDirection)
        {
            this.sceneToLoad.name = sceneToLoad;

            if (transitionType == TransitionType.Entrance)
                this.sceneToLoad.type = TransitionType.Exit;
            else
                this.sceneToLoad.type = TransitionType.Entrance;

            transitionDirection = moveDirection;
        }

        public async void BeginTransition()
        {
            // Set previous scene
            previousScene = SceneManager.GetActiveScene().name;

            isLoading = true;

            // Begin any transition effects
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onTransitionBegin.Raise(this, transitionDirection);

            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            // Begin any transition effects
            // Calls to:
            // - CameraFade.PlayFadeOut();
            onFadeOut.Raise(this, null);
        }

        public void ChangeScene() => SceneManager.LoadScene(sceneToLoad.name, LoadSceneMode.Single);

        public async void EndTransition()
        {
            await Task.Delay(TimeSpan.FromSeconds(transitionTime));

            isLoading = false;

            // End any transition effects
            // Calls to:
            //  -
            onTransitionEnd.Raise(this, null);
        }
    }
}
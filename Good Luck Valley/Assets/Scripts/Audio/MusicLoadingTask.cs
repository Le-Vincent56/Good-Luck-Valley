using Cysharp.Threading.Tasks;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class MusicLoadingTask : MonoBehaviour, ILoadingTask
    {
        private SceneLoader sceneLoader;

        [Header("Wwise States")]
        [SerializeField] private List<AK.Wwise.State> statesToSet;

        private void Awake()
        {
            // Get the scene loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        private void OnEnable()
        {
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            sceneLoader.QueryTasks += RegisterTask;
        }

        private void OnDisable()
        {
            sceneLoader.QueryTasks -= RegisterTask;
        }

        /// <summary>
        /// Create a task to play the cinematic
        /// </summary>
        /// <returns></returns>
        private UniTask PlayCinematic()
        {
            // Set each state
            MusicManager.Instance.SetStates(statesToSet);

            // Play the music
            MusicManager.Instance.Play();

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Register the task with the scene loader to be called after the loading screen and after the player has been placed
        /// </summary>
        public void RegisterTask() => sceneLoader.RegisterPostTask(PlayCinematic, 100);
    }
}

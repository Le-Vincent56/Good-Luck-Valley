using Cysharp.Threading.Tasks;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.World.Cinematics
{
    public class PostLoadCinematic : MonoBehaviour, ILoadingTask
    {
        private PlayableDirector director;
        private SceneLoader sceneLoader;

        private void Awake()
        {
            // Get components
            director = GetComponent<PlayableDirector>();

            // Get the scene loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        private void OnEnable()
        {
            if(sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

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
            // Play the director
            director.Play();

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Register the task with the scene loader to be called after the loading screen and after the player has been placed
        /// </summary>
        public void RegisterTask() => sceneLoader.RegisterPostTask(PlayCinematic, 100);
    }
}

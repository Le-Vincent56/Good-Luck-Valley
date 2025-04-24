using Cysharp.Threading.Tasks;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BaseTrigger : MonoBehaviour, ILoadingTask
    {
        protected SceneLoader sceneLoader;
        protected BoxCollider2D boxCollider;

        protected virtual void Awake()
        {
            // Get the box collider and disable it
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
        }

        protected virtual void OnEnable()
        {
            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            sceneLoader.QueryTasks += RegisterTask;
        }

        protected virtual void OnDisable()
        {
            sceneLoader.QueryTasks -= RegisterTask;
        }

        /// <summary>
        /// Enable the trigger
        /// </summary>
        public UniTask EnableTrigger()
        {
            // Enable the box collider
            boxCollider.enabled = true;

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Register the loading task
        /// </summary>
        public void RegisterTask() => sceneLoader.RegisterPostTask(EnableTrigger, 0);
    }
}

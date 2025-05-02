using Cinemachine;
using Cysharp.Threading.Tasks;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley
{
    public class CameraController : MonoBehaviour, ILoadingTask
    {
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;
        [SerializeField] private CinemachineVirtualCamera loadCamera;
        [SerializeField] private CinemachineVirtualCamera initialCamera;
        private SceneLoader sceneLoader;

        private void Awake()
        {
            virtualCameras = new List<CinemachineVirtualCamera>();

            // Get the virtual cameras
            GetComponentsInChildren(virtualCameras);

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void OnEnable()
        {
            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            sceneLoader.QueryTasks += RegisterTask;
        }

        private void OnDisable()
        {
            sceneLoader.QueryTasks -= RegisterTask;
        }

        /// <summary>
        /// Initially set a load camera to allow for cutting for the save handler
        /// </summary>
        private UniTask SetLoadCamera()
        {
            // Prioritize the load camera
            PrioritizeCamera(loadCamera);

            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            return UniTask.NextFrame();
        }

        private UniTask SetInitialCamera()
        {
            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            // Exit case - save camera data has been set
            if (sceneLoader.SetCameraData) return UniTask.NextFrame();

            // Prioritize the initial camera
            PrioritizeCamera(initialCamera);

            return UniTask.NextFrame();
        }

        /// <summary>
        /// Prioritize a Camera
        /// </summary>
        public void PrioritizeCamera(CinemachineVirtualCamera camToPrioritize)
        {
            // Exit case - if the camera to prioritize doesn't exist
            if (camToPrioritize == null) return;

            // Set the priority of the camera to higher
            camToPrioritize.Priority = 15;

            // Iterate through each virtual camera
            foreach (CinemachineVirtualCamera cam in virtualCameras)
            {
                // Check if the cameras are not equal
                if (camToPrioritize != cam)
                {
                    cam.Priority = 10;

                    continue;
                }
            }
        }

        /// <summary>
        /// Register the tasks to disable and enable camera blending during loading
        /// </summary>
        public void RegisterTask()
        {
            // Register the task to initialize the camera
            sceneLoader.RegisterPreTask(SetLoadCamera, 0);
            sceneLoader.RegisterPostTask(SetInitialCamera, 3);
        }
    }
}

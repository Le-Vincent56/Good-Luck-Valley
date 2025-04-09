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
        [SerializeField] private CinemachineVirtualCamera initialCamera;
        private CinemachineBlendDefinition originalBlend;
        private SceneLoader sceneLoader;


        private void Awake()
        {
            virtualCameras = new List<CinemachineVirtualCamera>();

            // Get the virtual cameras
            GetComponentsInChildren(virtualCameras);

            // Cache the original blend settings
            originalBlend = cinemachineBrain.m_DefaultBlend;

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

        private void Start()
        {
            // Iterate through each virtual camera
            foreach(CinemachineVirtualCamera cam in virtualCameras)
            {
                // Check if the camera is the initial camera
                if (cam == initialCamera)
                    // Make sure the initial camera has the highest priotity
                    cam.Priority = 15;
                else
                    // Otherwise, lower the priorities of the other cameras
                    cam.Priority = 10;
            }

            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
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
            // Register disable blends first
            sceneLoader.RegisterTask(DisableBlends(), 0);

            // Register enable blends third
            sceneLoader.RegisterTask(EnableBlends(), 2);
        }

        /// <summary>
        /// Disable the blends for loading
        /// </summary>
        public async UniTask DisableBlends()
        {
            // Set the default blend to a cut
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

            await UniTask.Delay(500, true);
        }

        /// <summary>
        /// Enable the blends after loading
        /// </summary>
        private async UniTask EnableBlends()
        {
            // Set the original blends
            cinemachineBrain.m_DefaultBlend = originalBlend;

            await UniTask.Delay(10, true);
        }
    }
}

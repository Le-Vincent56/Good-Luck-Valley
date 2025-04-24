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
        private CinemachineBlenderSettings originalBlendSettings;
        private SceneLoader sceneLoader;


        private void Awake()
        {
            virtualCameras = new List<CinemachineVirtualCamera>();

            // Get the virtual cameras
            GetComponentsInChildren(virtualCameras);

            // Cache the original blend settings
            originalBlend = cinemachineBrain.m_DefaultBlend;
            originalBlendSettings = cinemachineBrain.m_CustomBlends;

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

        private UniTask InitializeCamera()
        {
            // Prioritize the initial camera
            PrioritizeCamera(initialCamera);

            // Get the scene loader if it was not set
            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

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
            sceneLoader.RegisterPreTask(InitializeCamera, 0);

            // Register disable blends first
            sceneLoader.RegisterPreTask(DisableBlends, 2);

            // Register a blend buffer to allow for the camera to settle
            sceneLoader.RegisterPostTask(BlendBuffer, 1);

            // Register enable blends third
            sceneLoader.RegisterPostTask(EnableBlends, 2);
        }

        /// <summary>
        /// Disable the blends for loading
        /// </summary>
        public UniTask DisableBlends()
        {
            // Remove the blend settings
            cinemachineBrain.m_CustomBlends = null;

            // Set the default blend to a cut
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

            return UniTask.NextFrame();
        }

        /// <summary>
        /// Wait for a bit to allow the camera to settle
        /// </summary>
        private UniTask BlendBuffer() => UniTask.Delay(300, true);

        /// <summary>
        /// Enable the blends after loading
        /// </summary>
        private UniTask EnableBlends()
        {
            // Set the original blends
            cinemachineBrain.m_DefaultBlend = originalBlend;
            cinemachineBrain.m_CustomBlends = originalBlendSettings;

            return UniTask.NextFrame();
        }
    }
}

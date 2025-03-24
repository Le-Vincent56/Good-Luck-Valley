using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Persistence;
using GoodLuckValley.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley
{
    public class CameraController : MonoBehaviour
    {
        private SaveLoadSystem saveLoadSystem;

        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;
        [SerializeField] private CinemachineVirtualCamera initialCamera;
        private CinemachineBlendDefinition originalBlend;

        private CountdownTimer correctBlendTimer;

        private void Awake()
        {
            virtualCameras = new List<CinemachineVirtualCamera>();

            // Get the virtual cameras
            GetComponentsInChildren(virtualCameras);

            // Cache the original blend settings
            originalBlend = cinemachineBrain.m_DefaultBlend;

            correctBlendTimer = new CountdownTimer(0.2f);
            correctBlendTimer.OnTimerStop += () => EnableBlends();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);

            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
        }

        private void OnEnable()
        {
            // Add event listeners
           saveLoadSystem.Release += Cleanup;
           saveLoadSystem.PrepareDataBind += DisableBlends;
        }

        private void OnDisable()
        {
            // Cleanup
            Cleanup();
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
        }

        private void OnDestroy()
        {
            // Dispose of the correct blend timer
            correctBlendTimer?.Dispose();
        }

        /// <summary>
        /// Clean up by removing the event listeners
        /// </summary>
        private void Cleanup()
        {
            saveLoadSystem.Release -= Cleanup;
            saveLoadSystem.PrepareDataBind -= DisableBlends;
        }

        /// <summary>
        /// Disable the Cinemachine Brain's blends
        /// </summary>
        private void DisableBlends()
        {
            // Set the default blend to a cut
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

            // Start the correct blend timer
            correctBlendTimer.Start();
        }

        private void EnableBlends()
        {
            // Set the original blends
            cinemachineBrain.m_DefaultBlend = originalBlend;
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
    }
}

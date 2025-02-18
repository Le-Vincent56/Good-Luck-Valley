using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
        [SerializeField] private CinemachineVirtualCamera initialCamera;

        private void Awake()
        {
            // Get the virtual cameras
            virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);
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

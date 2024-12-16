using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

        private void Awake()
        {
            // Get the virtual cameras
            virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        /// <summary>
        /// Prioritize a Camera
        /// </summary>
        public void PrioritizeCamera(CinemachineVirtualCamera camToPrioritize)
        {
            // Set the priority of the camera to higher
            camToPrioritize.Priority = 10;

            // Iterate through each virtual camera
            foreach (CinemachineVirtualCamera cam in virtualCameras)
            {
                // Check if the cameras are not equal
                if (camToPrioritize != cam)
                {
                    // Set to a lower priority
                    cam.Priority = 5;

                    continue;
                }
            }
        }
    }
}

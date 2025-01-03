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
            initialCamera.gameObject.SetActive(true);
        }

        /// <summary>
        /// Prioritize a Camera
        /// </summary>
        public void PrioritizeCamera(CinemachineVirtualCamera camToPrioritize)
        {
            // Exit case - if the camera to prioritize doesn't exist
            if (camToPrioritize == null) return;

            // Set the priority of the camera to higher
            camToPrioritize.gameObject.SetActive(true);

            // Iterate through each virtual camera
            foreach (CinemachineVirtualCamera cam in virtualCameras)
            {
                // Check if the cameras are not equal
                if (camToPrioritize != cam)
                {
                    cam.gameObject.SetActive(false);

                    continue;
                }
            }
        }
    }
}

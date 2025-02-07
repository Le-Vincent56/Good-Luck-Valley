using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class OneWayCameraTrigger : BaseTrigger
    {
        private CameraController cameraController;
        [SerializeField] private CinemachineVirtualCamera cameraToPrioritize;

        private void Start()
        {
            // Get the Camera Controller as a servicve
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collider does not have a PlayerController attached
            if (!collision.GetComponent<PlayerController>()) return;

            // Prioritize the camera
            cameraController.PrioritizeCamera(cameraToPrioritize);
        }
    }
}

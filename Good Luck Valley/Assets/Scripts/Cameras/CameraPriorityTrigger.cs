using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraPriorityTrigger : MonoBehaviour
    {
        private CameraController cameraController;
        [SerializeField] private CinemachineVirtualCamera cameraToPrioritize;

        private void Start()
        {
            // Get the Camera Controller as a servicve
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the collider does not have a PlayerController attached
            if (!collision.GetComponent<PlayerController>()) return;

            // Prioritize the camera
            cameraController.PrioritizeCamera(cameraToPrioritize);
        }
    }
}

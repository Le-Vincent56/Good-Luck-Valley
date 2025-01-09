using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TwoWayCameraTrigger : MonoBehaviour
    {
        private CameraController cameraController;
        private Vector3 center;

        [SerializeField] private CinemachineVirtualCamera leftCamera;
        [SerializeField] private CinemachineVirtualCamera rightCamera;

        private void Start()
        {
            // Get the Camera Controller as a servicve
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();

            // Set the center of the trigger
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            center = bounds.center;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collider does not have a PlayerController attached
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Get the direction from the controller to the player
            Vector2 exitDirection = controller.transform.position - center;
            int enterDirectionX = (int)Mathf.Sign(exitDirection.x);

            // Check if exiting from the right
            if (enterDirectionX == 1)
                // Prioritize the right camera
                cameraController.PrioritizeCamera(rightCamera);
            // Else, check if exiting from the left
            else if (enterDirectionX == -1)
                cameraController.PrioritizeCamera(leftCamera);
        }
    }
}

using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Cameras.Triggers
{
    public class TwoWayActivateableCameraTrigger : ActivateableTrigger
    {
        protected Vector3 center;
        private CameraController cameraController;
        [SerializeField] private CinemachineVirtualCamera leftCamera;
        [SerializeField] private CinemachineVirtualCamera rightCamera;

        protected virtual void Awake()
        {
            // Set the center of the trigger
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            center = bounds.center;
        }

        private void Start()
        {
            // Get the Camera Controller as a servicve
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collider does not have a PlayerController attached
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Get the direction from the controller to the player
            Vector2 exitDirection = controller.transform.position - center;
            int enterDirectionX = (int)Mathf.Sign(exitDirection.x);

            // Check if exiting from the right
            if (enterDirectionX == 1)
                // Prioritize the right function
                OnRight();
            // Else, check if exiting from the left
            else if (enterDirectionX == -1)
                OnLeft();
        }

        /// <summary>
        /// Prioritize the right camera
        /// </summary>
        private void OnRight()
        {
            // Exit case - if not active
            if (!active) return;

            cameraController.PrioritizeCamera(rightCamera);
        }

        /// <summary>
        /// Prioritize the left camera
        /// </summary>
        private void OnLeft()
        {
            // Exit case - if not active
            if (!active) return;

            cameraController.PrioritizeCamera(leftCamera);
        }
    }
}

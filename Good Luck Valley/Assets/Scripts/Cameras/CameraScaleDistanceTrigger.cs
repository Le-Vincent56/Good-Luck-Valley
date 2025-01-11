using Cinemachine;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraScaleDistanceTrigger : FloatDistanceBasedTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            // Calculate through the parent class
            base.OnTriggerStay2D(collision);

            // Set the Orthographic Size using the current distance value
            virtualCamera.m_Lens.OrthographicSize = currentDistanceValue;
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collision is not the Player
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            float exitValue = currentDistanceValue;

            // Get the direction from the controller to the player
            Vector2 exitVector = controller.transform.position - center;
            int exitDirection;

            // Determine the exit value based on the direction of the trigger
            switch (direction)
            {
                case Direction.Left:
                    exitDirection = (int)Mathf.Sign(exitVector.x);
                    exitValue = CalculateExitValue(exitDirection, 1, - 1);
                    break;

                case Direction.Down:
                    exitDirection = (int)Mathf.Sign(exitVector.y);
                    exitValue = CalculateExitValue(exitDirection, 1, -1);
                    break;

                case Direction.Right:
                    exitDirection = (int)Mathf.Sign(exitVector.x);
                    exitValue = CalculateExitValue(exitDirection, -1, 1);
                    break;

                case Direction.Up:
                    exitDirection = (int)Mathf.Sign(exitVector.y);
                    exitValue = CalculateExitValue(exitDirection, -1, 1);
                    break;

                default:
                    exitValue = currentDistanceValue;
                    break;
            }

            // Set the Orthographic Size to the exit value
            virtualCamera.m_Lens.OrthographicSize = exitValue;
        }
    }
}

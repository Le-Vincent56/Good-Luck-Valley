using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public abstract class TwoWayTrigger : BaseTrigger
    {
        protected Vector3 center;

        protected virtual void Awake()
        {
            // Set the center of the trigger
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            center = bounds.center;
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

        protected abstract void OnLeft();
        protected abstract void OnRight();
    }
}

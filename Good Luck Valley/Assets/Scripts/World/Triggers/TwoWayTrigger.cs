using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public abstract class TwoWayTrigger : BaseTrigger
    {
        protected enum Alignment
        {
            None,
            Vertical,
            Horizontal
        }

        protected Vector3 center;
        protected BoxCollider2D boxCollider;
        [SerializeField] protected Alignment alignment;

        protected override void Awake()
        {
            // Get components
            boxCollider = GetComponent<BoxCollider2D>();

            // Set the center of the trigger
            Bounds bounds = boxCollider.bounds;
            center = bounds.center;
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collider does not have a PlayerController attached
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            Vector2 exitDirection;
            int enterDirection;

            if (alignment == Alignment.Vertical)
            {
                // Get the direction from the controller to the player
                exitDirection = controller.transform.position - center;
                enterDirection = (int)Mathf.Sign(exitDirection.y);

                // Check if exiting from the right
                if (enterDirection == 1)
                    // Prioritize the up function
                    OnUp();
                // Else, check if exiting from downward
                else if (enterDirection == -1)
                    OnDown();
            }

            // Get the direction from the controller to the player
            exitDirection = controller.transform.position - center;
            enterDirection = (int)Mathf.Sign(exitDirection.x);

            // Check if exiting from the right
            if (enterDirection == 1)
                // Prioritize the right function
                OnRight();
            // Else, check if exiting from the left
            else if (enterDirection == -1)
                OnLeft();
        }

        protected virtual void OnLeft() { }
        protected virtual void OnRight() { }
        protected virtual void OnUp() { }
        protected virtual void OnDown() { }
    }
}

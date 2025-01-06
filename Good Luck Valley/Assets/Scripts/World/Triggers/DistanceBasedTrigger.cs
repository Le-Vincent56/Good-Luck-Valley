using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class DistanceBasedTrigger : MonoBehaviour
    {
        protected enum Direction
        {
            None,
            Left,
            Right,
            Up,
            Down
        }

        [SerializeField] protected Direction direction;
        [SerializeField] protected float target;
        [SerializeField] protected float totalDistance;

        protected virtual void Awake()
        {
            // Get the bounds of the BoxCollider2D
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;

            // Get the total distance bounds
            totalDistance = bounds.size.x;

            // Calculate the target value depending on the Scale Direction
            switch (direction)
            {
                case Direction.Left:
                    target = bounds.min.x;
                    break;

                case Direction.Right:
                    target = bounds.max.x;
                    break;

                case Direction.Up:
                    target = bounds.max.y;
                    break;

                case Direction.Down:
                    target = bounds.min.y;
                    break;

                case Direction.None:
                    target = 0;
                    break;
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - the collision is not the Player
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Create a container for the current distance
            float currentDistance = 0;

            // Check if the Scale Direction is horizontal
            if (direction is Direction.Left || direction is Direction.Right)
                // Calculate the current distance using the player's x-value
                currentDistance = Mathf.Abs(controller.transform.position.x - target);
            // Otherwise, check if the Scale Direction is vertical
            else if (direction is Direction.Up || direction is Direction.Down)
                // Calculate the current distance using the player's y-value
                currentDistance = Mathf.Abs(controller.transform.position.y - target);
            // Otherwise, exit
            else return;

            // Calculate the distance value
            CalculateDistanceValue(currentDistance);
        }

        protected abstract void CalculateDistanceValue(float currentDistance);
    }
}

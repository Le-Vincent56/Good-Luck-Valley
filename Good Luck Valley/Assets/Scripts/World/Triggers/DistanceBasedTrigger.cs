using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public abstract class DistanceBasedTrigger : BaseTrigger
    {
        protected enum Direction
        {
            None,
            Left,
            Right,
            Up,
            Down
        }

        protected enum EaseType
        {
            Linear,
            InOutSine,
            InOutQuad,
            InQuad,
            OutQuad
        }

        [SerializeField] protected Direction direction;
        [SerializeField] protected EaseType easeType;
        [SerializeField] protected float target;
        [SerializeField] protected float totalDistance;
        protected Vector3 center;

        protected virtual void Awake()
        {
            // Get the bounds of the BoxCollider2D
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;

            // Get the total distance bounds
            center = bounds.center;

            // Calculate the target value depending on the Scale Direction
            switch (direction)
            {
                case Direction.Left:
                    target = bounds.min.x;
                    totalDistance = bounds.size.x;
                    break;

                case Direction.Right:
                    target = bounds.max.x;
                    totalDistance = bounds.size.x;
                    break;

                case Direction.Up:
                    target = bounds.max.y;
                    totalDistance = bounds.size.y;
                    break;

                case Direction.Down:
                    target = bounds.min.y;
                    totalDistance = bounds.size.y;
                    break;

                case Direction.None:
                    target = 0;
                    break;
            }
        }

        protected virtual void OnDestroy() { /* Noop */}

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

        /// <summary>
        /// Apply easing to a t-value
        /// </summary>
        protected float ApplyEasing(float t)
        {
            return easeType switch
            {
                EaseType.Linear => t,
                EaseType.InOutSine => EaseInOutSine(t),
                EaseType.InOutQuad => EaseInOutQuad(t),
                EaseType.InQuad => EaseInQuad(t),
                EaseType.OutQuad => EaseOutQuad(t),
                _ => t,
            };
        }

        private float EaseInOutSine(float t)
        {
            return 0.5f * (1 - Mathf.Cos(Mathf.PI * t));
        }

        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        private float EaseInQuad(float t)
        {
            return t * t;
        }

        private float EaseOutQuad(float t)
        {
            return t * (2 - t);
        }
    }
}

using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class FireflyLocomotionState : FireflyState
    {
        private Vector2 offset;

        private readonly float maxSpeed;
        private readonly float acceleration;
        private readonly float decelerationDistance;

        private Vector2 targetPosition;
        private float currentSpeed;

        public FireflyLocomotionState(Firefly firefly, float maxSpeed, float acceleration, float decelerationDistance) : base(firefly)
        {
            // Set variables
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.decelerationDistance = decelerationDistance;
            currentSpeed = 0f;
        }

        public override void OnEnter()
        {
            // Get a random offset within 
            offset = firefly.Group.GetRandomOffset();
        }

        public override void Update()
        {
            // Update the target position to the offset position within the circle
            targetPosition = firefly.Group.GetOffsetPosition(offset);

            // Get the distance to the target position
            float distanceToTarget = Vector2.Distance(firefly.transform.position, targetPosition);

            if (distanceToTarget < decelerationDistance)
            {
                // Decelerate to the current speed
                float t = distanceToTarget / decelerationDistance;
                currentSpeed = Mathf.Lerp(0, maxSpeed, t);
            }
            else
            {
                // Accelerate the current speed
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            }

            // Move the firefly to the target position
            firefly.transform.position = Vector2.MoveTowards(
                firefly.transform.position,
                targetPosition,
                currentSpeed * Time.deltaTime
            );

            if (Vector2.Distance(firefly.transform.position, targetPosition) < 0.01f)
            {
                // Set the current speed to 0
                currentSpeed = 0f;

                // Notify that the firefly has reached its destination
                firefly.Wandering = true;
            }
        }
    }
}

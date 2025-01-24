using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class FireflyWanderState : FireflyState
    {
        private readonly float maxSpeed;
        private readonly float acceleration;
        private readonly float decelerationDistance;

        private Vector2 targetPosition;
        private float currentSpeed;
        private float bounceOffset;
        private float bounceSpeed;
        private float bounceAmplitude;

        public FireflyWanderState(Firefly firefly, float maxSpeed, float acceleration, float decelerationDistance, float bounceSpeed, float bounceAmplitude) : base(firefly)
        {
            // Set variables
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.decelerationDistance = decelerationDistance;
            this.bounceSpeed = bounceSpeed;
            this.bounceAmplitude = bounceAmplitude;
            currentSpeed = 0f;
        }

        public override void OnEnter()
        {
            // Get a new position
            targetPosition = firefly.Group.GetRandomPositionInCircle();

            // Reset the bounce offset 
            bounceOffset = 0f;
        }

        public override void Update()
        {
            // Calculate the bounce effect
            bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceAmplitude;

            // Calculate the direction to the target position
            Vector2 currentPosition = firefly.transform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;

            // Calculate the distance to the target position
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

            // Check if within deceleration distance
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
            Vector2 movement = currentSpeed * Time.deltaTime * direction;
            firefly.transform.position += (Vector3)movement;

            // Apply the bounce effect
            firefly.transform.position += Vector3.up * bounceOffset;

            // Check if the Firefly has reached its target position
            if(distanceToTarget <= 0.1f)
            {
                // Get a new position
                targetPosition = firefly.Group.GetRandomPositionInCircle();
            }
        }
    }
}

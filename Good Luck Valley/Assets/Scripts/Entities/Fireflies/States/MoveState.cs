using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies.States
{
    public class MoveState : FireflyState
    {
        private float maxSpeed;
        private float acceleration;
        private float decelerationDistance;

        private Vector2 targetPosition;
        private float currentSpeed;

        public MoveState(Firefly firefly, float maxSpeed, float acceleration, float decelerationDistance) : base(firefly)
        {
            // Set variables
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.decelerationDistance = decelerationDistance;
            currentSpeed = 0f;
        }

        public override void OnEnter()
        {
            // Get a new position
            targetPosition = firefly.GetNewPosition();
        }

        public override void Update()
        {
            // Get the distance to the target position
            float distanceToTarget = Vector2.Distance(firefly.transform.position, targetPosition);

            if(distanceToTarget < decelerationDistance)
            {
                // Decelerate to the current speed
                float t = distanceToTarget / decelerationDistance;
                currentSpeed = Mathf.Lerp(0, maxSpeed, t);
            } else
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
                firefly.SetToMove(false);
            }
        }
    }
}
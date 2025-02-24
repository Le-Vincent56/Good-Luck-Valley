using GoodLuckValley.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Firefly : MonoBehaviour
    {
        [Header("Physics Vectors and Mass")]
        [SerializeField] private Vector2 position = Vector2.zero;
        [SerializeField] private Vector2 velocity = Vector2.zero;
        [SerializeField] private Vector2 acceleration = Vector2.zero;
        [SerializeField] private Vector2 totalForce = Vector2.zero;
        [SerializeField] private float mass = 1f;

        [Header("Movement")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float bounceOffset;
        [SerializeField] private float personalSpace;
        [SerializeField] private float wanderTime;
        private float bounceSpeed;
        private float bounceAmplitude;
        private CountdownTimer wanderTimer;

        private Vector2 targetPosition;

        public Vector2 Position { get => position; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float PersonalSpace { get => personalSpace; set => personalSpace = value; }
        public float WanderTime { get => wanderTime; set => wanderTime = value; }

        /// <summary>
        /// Initialize the Firefly within the Firefly Group
        /// </summary>
        public void Initialize(FireflyGroup group)
        {
            // Set variables
            targetPosition = group.GetRandomPositionInCircle();
            bounceSpeed = 10f;
            bounceAmplitude = -0.00075f;
            position = transform.position;

            // Set the Wander timer
            wanderTimer = new CountdownTimer(Random.Range(0.5f, 1.5f));
            wanderTimer.OnTimerStop += () =>
            {
                targetPosition = group.GetRandomPositionInCircle();
                wanderTimer.Start();
            };

            // Start the wander timer
            wanderTimer.Start();
        }

        /// <summary>
        /// Update the Firefly
        /// </summary>
        public void TickUpdate(float time, float delta, List<Firefly> fireflies)
        {
            // Get the current transform position
            position = transform.position;
            
            // Calcualte the forces
            CalculateForces(fireflies);

            // Apply force
            acceleration += totalForce / mass;

            // Calculate the velocity
            velocity += acceleration * delta;

            // Calculate the position
            position += velocity * delta;

            // Calculate the bounce effect
            bounceOffset = Mathf.Sin(time * bounceSpeed) * -bounceAmplitude;

            // Apply the bounce effect
            position += Vector2.up * bounceOffset;

            // Set the final position
            transform.position = position;

            // Zero out acceleration and total force
            acceleration = Vector2.zero;
            totalForce = Vector2.zero;
        }

        /// <summary>
        /// Calculate the total affecting forces for the Firefly
        /// </summary>
        private void CalculateForces(List<Firefly> fireflies)
        {
            totalForce += Seek(targetPosition);
            totalForce += StayCohesive(fireflies);
            totalForce += Separate(fireflies);
        }

        /// <summary>
        /// Seek a target position
        /// </summary>
        private Vector2 Seek(Vector2 target, float weight = 1f)
        {
            // Calculate the desired velocity and scale by max speed
            Vector2 desiredVelocity = target - position;
            desiredVelocity = desiredVelocity.normalized * maxSpeed;

            // Calculate and return the steering force
            Vector2 seekingForce = desiredVelocity - velocity;
            return seekingForce * weight;
        }

        /// <summary>
        /// Flee from a target position
        /// </summary>
        private Vector2 Flee(Vector2 target, float weight = 1f)
        {
            // Calculate desired velocity and scale by max speed
            Vector2 desiredVelocity = position - target;
            desiredVelocity = desiredVelocity.normalized * maxSpeed;

            // Calculate and return steering force
            Vector2 fleeingForce = desiredVelocity - velocity;
            return fleeingForce * weight;
        }

        /// <summary>
        /// Stay cohesive with the Firefly Group
        /// </summary>
        private Vector2 StayCohesive(List<Firefly> fireflies, float weight = 0.25f)
        {
            // Get the center
            Vector2 centroid = Vector2.zero;

            foreach(Firefly firefly in fireflies)
            {
                // Add the firefly's position to the centroid
                centroid += firefly.position;
            }

            // Divide the centroid by the number of fireflies
            centroid /= fireflies.Count;

            // Seek the centroid
            return Seek(centroid, weight);
        }

        /// <summary>
        /// Space out from the other Fireflies
        /// </summary>
        private Vector2 Separate(List<Firefly> fireflies)
        {
            Vector2 separationForce = Vector2.zero;
            float sqrPersonalSpace = Mathf.Pow(personalSpace, 2f);

            // Iterate through each Firefly
            foreach (Firefly firefly in fireflies)
            {
                // Skip if the Firefly is the same as this Firefly
                if (firefly == this) continue;

                // Calculate the square distance between this Firefly and the other Firefly
                float sqrDist = Vector2.SqrMagnitude(firefly.Position - position);

                // Skip if the distance is negligible
                if (sqrDist < float.Epsilon) continue;

                // Skip if the distance is greater than the personal space
                if (sqrDist >= sqrPersonalSpace) continue;

                // Calcualte the separation force
                float weight = sqrPersonalSpace / (sqrDist + 0.1f);
                separationForce += Flee(firefly.Position, weight);
            }

            return separationForce;
        }
    }
}

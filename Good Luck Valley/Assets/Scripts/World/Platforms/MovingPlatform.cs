using GoodLuckValley.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Platforms
{
    public class MovingPlatform : Raycaster
    {
        [SerializeField] private LayerMask passengerMask;
        [SerializeField] private Vector3 move;

        private void Update()
        {
            // Update raycasts
            UpdateRaycastOrigins();

            // Calculate velocity
            Vector3 velocity = move * Time.deltaTime;

            // Move passengers
            MovePassengers(velocity);

            // Move the platform
            transform.Translate(velocity);
        }

        private void MovePassengers(Vector2 velocity)
        {
            // Get the passengers being moved
            HashSet<Transform> movedPassengers = new HashSet<Transform>();

            // Get the direction components of movement
            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Handle vertically moving platforms
            if (velocity.y != 0)
            {
                TravelOnY(movedPassengers, directionX, directionY, velocity);
            }

            // Handle horizontally moving platforms
            if (velocity.x != 0)
            {
                // Get the ray length
                float rayLength = Mathf.Abs(velocity.x) + skinWidth;

                // Loop through the horizontal rays
                for (int i = 0; i < horizontalRayCount; i++)
                {
                    // Cast a ray to any passengers
                    Vector2 rayOrigin = (directionX == -1) ? origins.bottomLeft : origins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                    // Check if a passenger is hit
                    if (hit)
                    {
                        // Check if the hashset contains the passenger transform,
                        // this prevents a passenger from being moved multiple
                        // times per frame
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            // Add the passenger
                            movedPassengers.Add(hit.transform);

                            // Calculate push forces
                            float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                            float pushY = 0f;

                            // Transform the passenger
                            hit.transform.Translate(new Vector3(pushX, pushY));
                        }

                    }
                }
            }

            // Handle passengers on top of a horizontally-moving or downward-moving platform
            if(directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                TravelOnY(movedPassengers, directionX, directionY, velocity);
            }
        }

        private void TravelOnY(HashSet<Transform> passengers, float directionX, float directionY, Vector2 velocity)
        {
            // Get the ray length
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            // Loop through the vertical rays
            for (int i = 0; i < verticalRayCount; i++)
            {
                // Cast a ray to any passengers
                Vector2 rayOrigin = (directionY == -1) ? origins.bottomLeft : origins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                // Check if a passenger is hit
                if (hit)
                {
                    // Check if the hashset contains the passenger transform,
                    // this prevents a passenger from being moved multiple
                    // times per frame
                    if (!passengers.Contains(hit.transform))
                    {
                        // Add the passenger
                        passengers.Add(hit.transform);

                        // Calculate push forces
                        float pushX = (directionY == 1) ? velocity.x : 0f;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        // Transform the passenger
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }

                }
            }
        }
    }
}
using GoodLuckValley.Entity;
using GoodLuckValley.Player.Control;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Platforms
{
    public class MovingPlatform : Raycaster
    {
        [Serializable]
        struct PassengerMovement
        {
            public Transform Transform;
            public Vector2 Velocity;
            public bool StandingOnPlatform;
            public bool MoveBeforePlatform;

            public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool moveBeforePlatform)
            {
                Transform = transform;
                Velocity = velocity;
                StandingOnPlatform = standingOnPlatform;
                MoveBeforePlatform = moveBeforePlatform;
            }
        }

        [Header("Fields - Details")]
        [SerializeField] private LayerMask passengerMask;
        [SerializeField] private float platformSpeed;
        [SerializeField] private float waitTime;
        [SerializeField] [Range(0, 2)] private float easeAmount;
        private float nextMoveTime;
        private int fromWaypointIndex;
        private float percentBetweenWaypoints;

        [Header("Fields - Waypoints")]
        [SerializeField] private bool cyclic;
        [SerializeField] private Vector2[] localWaypoints;
        [SerializeField] private Vector2[] globalWaypoints;


        [SerializeField] private List<PassengerMovement> passengerMovements;
        Dictionary<Transform, PlayerController> passengerDictionary = new Dictionary<Transform, PlayerController>();

        private void Start()
        {
            // Convert local waypoints to global waypoints
            globalWaypoints = new Vector2[localWaypoints.Length];
            for(int i = 0; i < localWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + (Vector2)transform.position;
            }
        }

        private void Update()
        {
            // Update raycasts
            UpdateRaycastOrigins();

            // Calculate velocity
            Vector3 velocity = CalculatePlatformMovement();

            // Calculate movement for passengers
            CalculatePassengerMovement(velocity);

            // Move passengers before platform
            MovePassengers(true);

            // Move the platform
            transform.Translate(velocity);

            // Move passengers after platform
            MovePassengers(false);
        }

        private Vector3 CalculatePlatformMovement()
        {
            // Check the platform can move
            if(Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }

            // Reset to 0 when we reach the length of the array
            fromWaypointIndex %= globalWaypoints.Length;

            // Get which waypoint to move to next
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

            // Calculate the distance between the two waypoints
            float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);

            // Calculate the percentage to the next waypoint (divide by distance to allow for normalized speed)
            percentBetweenWaypoints += Time.deltaTime * platformSpeed/distanceBetweenWaypoints;

            // Ease the movement
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            // Get the new position
            Vector2 newPos = Vector2.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            // Check if we have reached the waypoint
            if(percentBetweenWaypoints >= 1)
            {
                // Reset the percentage
                percentBetweenWaypoints = 0;

                // Increment the index to notify that we've reached the waypoint
                fromWaypointIndex++;

                // Check if the movement is cyclic
                if(!cyclic)
                {
                    // Check if we've reached the end of the waypoints
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        // Reset the index
                        fromWaypointIndex = 0;

                        // Reverse the array
                        Array.Reverse(globalWaypoints);
                    }
                }

                // Wait before moving
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - (Vector2)transform.position;
        }

        private float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        private void CalculatePassengerMovement(Vector2 velocity)
        {
            // Get the passengers being moved
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            passengerMovements = new List<PassengerMovement>();

            // Get the direction components of movement
            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Handle vertically moving platforms
            if (velocity.y != 0)
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
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            // Add the passenger
                            movedPassengers.Add(hit.transform);

                            // Calculate push forces
                            float pushX = (directionY == 1) ? velocity.x : 0f;
                            float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                            // Transform the passenger
                            // Transform the passenger
                            passengerMovements.Add(new PassengerMovement(
                                hit.transform,
                                new Vector3(pushX, pushY),
                                directionY == 1,
                                true // Move passenger first
                            ));
                        }
                    }
                }
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

                            // Ensure the passenger realizes it's on the ground
                            float pushY = -skinWidth;

                            // Transform the passenger
                            passengerMovements.Add(new PassengerMovement(
                                hit.transform,
                                new Vector3(pushX, pushY),
                                false,
                                true // Move passenger first
                            ));
                        }

                    }
                }
            }

            // Handle passengers on top of a horizontally-moving or downward-moving platform
            if(directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                // Get the ray length
                float rayLength = skinWidth * 2;

                // Loop through the vertical rays
                for (int i = 0; i < verticalRayCount; i++)
                {
                    // Cast a ray to any passengers
                    Vector2 rayOrigin = origins.topLeft + Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

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
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            // Transform the passenger
                            passengerMovements.Add(new PassengerMovement(
                                hit.transform,
                                new Vector3(pushX, pushY),
                                true,
                                false // Move platform first
                            ));
                        }

                    }
                }
            }
        }
        
        private void MovePassengers(bool beforeMovePlatform)
        {
            // Loop through each passenger
            foreach(PassengerMovement passenger in passengerMovements)
            {
                // Check if the passenger is in the dictionary
                if(!passengerDictionary.ContainsKey(passenger.Transform))
                {
                    // If not, add it - prevents us from doing so many GetComponent() calls
                    passengerDictionary.Add(passenger.Transform, passenger.Transform.GetComponent<PlayerController>());
                }

                // Check if we need to move the passenger before the platform
                if(passenger.MoveBeforePlatform == beforeMovePlatform)
                {
                    // Move the passenger
                    passengerDictionary[passenger.Transform].Move(passenger.Velocity, passenger.StandingOnPlatform);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if(localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = 0.1f;

                for(int i = 0; i < localWaypoints.Length; i++)
                {
                    // Convert local position to global position
                    Vector2 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + (Vector2)transform.position;

                    // Draw a circle for points
                    Gizmos.DrawSphere(globalWaypointPos, size);
                }
            }
        }
    }
}
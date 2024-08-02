using GoodLuckValley.Player.Control;
using GoodLuckValley.World.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Entity
{
    public enum CollisionLayer
    {
        None = 0,
        Ground = 3,
        Wall = 6,
        MushroomWall = 7,
        Slope = 8,
        Mushroom = 9,
        Player = 10,
        Decomposable = 12,
    }

    [Serializable]
    public class DynamicCollisionHandler : Raycaster
    {
        public struct CollisionInfo
        {
            public bool Above, Below;
            public bool Left, Right;
            public bool ClimbingSlope;
            public bool DescendingSlope;
            public float SlopeAngle, PrevSlopeAngle;
            public int SlopeDescentDirection;
            public Vector2 PrevVelocity;
            public int FacingDirection;
            public bool SlidingDownMaxSlope;
            public Vector2 SlopeNormal;
            public Vector2 LastSeenSlopeNormal;
            public int PrevLastSlopeVerticalDirection;
            public int LastSlopeVerticalDirection;
            public CollisionLayer Layer;
            public RaycastHit2D VerticalCollisionRay;
            public RaycastHit2D HorizontalCollisionRay;
            public bool CameraWithinGroundDistance;
            public bool CanStand;

            /// <summary>
            /// Reset the Collision Info
            /// </summary>
            public void ResetInfo()
            {
                Above = Below = false;
                Left = Right = false;
                ClimbingSlope = false;
                DescendingSlope = false;
                PrevSlopeAngle = SlopeAngle;
                PrevLastSlopeVerticalDirection = LastSlopeVerticalDirection;
                SlopeAngle = 0f;
                SlopeNormal = Vector2.zero;
                SlidingDownMaxSlope = false;
                Layer = CollisionLayer.Ground;
                VerticalCollisionRay = new RaycastHit2D();
                HorizontalCollisionRay = new RaycastHit2D();
                CameraWithinGroundDistance = false;
                CanStand = true;
            }
        }

        [Header("Fields")]
        [SerializeField] private bool debug;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float maxSlopeAngle;
        [SerializeField] private CollisionLayer currentLayer;
        [SerializeField] public CollisionInfo collisions;
        private Dictionary<int, CollisionLayer> layers;

        private void Start()
        {
            collisions.FacingDirection = 1;
            layers = new Dictionary<int, CollisionLayer>()
            {
                {0, CollisionLayer.None },
                {3, CollisionLayer.Ground},
                {6, CollisionLayer.Wall},
                {7, CollisionLayer.MushroomWall},
                {8, CollisionLayer.Slope},
                {9, CollisionLayer.Mushroom },
                {10, CollisionLayer.Player },
                {12, CollisionLayer.Decomposable }
            };
        }

        public void PredictGround(Vector2 velocity, float predictAmount)
        {
            // Set direction and ray length
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + predictAmount + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? origins.bottomLeft : origins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hitCollider = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                // Debug
                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hitCollider)
                {
                    // Set the ray length equal to the hit distance
                    rayLength = hitCollider.distance;

                    // Set collision info
                    collisions.CameraWithinGroundDistance = true;
                }
            }
        }

        public void CheckCanStand(Vector2 velocity, float standCheckDist)
        {
            // Set direction and ray length
            float rayLength = Mathf.Abs(velocity.y) + (standCheckDist) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = origins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hitCollider = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask);

                if(debug)
                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.blue);

                if (hitCollider)
                {
                    // Set the ray length equal to the hit distance
                    rayLength = hitCollider.distance;

                    // Set collision info
                    collisions.CanStand = false;
                }
            }
        }


        /// <summary>
        /// Handle vertical collisions
        /// </summary>
        /// <param name="velocity">The current velocity</param>
        public void VerticalCollisions(ref Vector2 velocity)
        {
            // Set direction and ray length
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            // Prepare to find the center-most collision point
            bool centralRayHit = false;
            RaycastHit2D centralHitRay = new RaycastHit2D();
            List<RaycastHit2D> hitRays = new List<RaycastHit2D>();

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? origins.bottomLeft : origins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hitCollider = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                // Debug
                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hitCollider)
                {
                    // Adjust the velocity y
                    velocity.y = (hitCollider.distance - skinWidth) * directionY;

                    // Set the ray length equal to the hit distance
                    rayLength = hitCollider.distance;

                    if(collisions.ClimbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }

                    hitRays.Add(hitCollider);

                    if(i == centralVerticalRay)
                    {
                        centralRayHit = true;
                        centralHitRay = hitCollider;
                    }    

                    // Set collision info
                    collisions.Above = (directionY == 1);
                    collisions.Below = (directionY == -1);
                    collisions.Layer = layers[hitCollider.transform.gameObject.layer];
                    currentLayer = collisions.Layer;

                    // Reset the last seen slope variables if detecting ground
                    if(collisions.Layer == CollisionLayer.Ground)
                    {
                        collisions.LastSeenSlopeNormal = Vector2.zero;
                        collisions.LastSlopeVerticalDirection = 0;
                    }
                        
                } else if(!collisions.Above && !collisions.Below)
                {
                    currentLayer = CollisionLayer.None;
                }
            }

            // Check if the central ray was hit
            if (centralRayHit)
            {
                collisions.VerticalCollisionRay = centralHitRay;
            } else if (hitRays.Count > 0)
            {
                // Set the max value for distance to compare
                float closestDistance = float.MaxValue;

                // Iterate through each point and find the closest distance to the center
                foreach (RaycastHit2D ray in hitRays)
                {
                    // Compare the distance
                    float distance = Mathf.Abs(ray.point.x - (origins.bottomLeft.x + (origins.topRight.x - origins.bottomLeft.x) / 2));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        collisions.VerticalCollisionRay = ray;
                    }
                }
            }

            // Debug
            if (debug)
                Debug.DrawRay(collisions.VerticalCollisionRay.point, Vector2.up * directionY, Color.white);

            // If climbing a slope, fire out a new ray in the future position
            // To check for a new slope
            if(collisions.ClimbingSlope)
            {
                // Create and send the ray cast
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? origins.bottomLeft : origins.bottomRight) + Vector2.up * velocity.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                // Check if it hits
                if(hit)
                {
                    // Get the slope angle
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    // Compare the slope angle with the current slope angle
                    if(slopeAngle != collisions.SlopeAngle)
                    {
                        // Adjust the x-velocity for the new slope
                        velocity.x = (hit.distance - skinWidth) * directionX;

                        // Set the new slope
                        collisions.SlopeAngle = slopeAngle;
                        collisions.Layer = layers[hit.transform.gameObject.layer];
                        collisions.LastSlopeVerticalDirection = 1;
                        currentLayer = collisions.Layer;
                    }
                }
            }
        }

        /// <summary>
        /// Handle horizontal collisions
        /// </summary>
        /// <param name="velocity">The current velocity</param>
        public void HorizontalCollisions(ref Vector2 velocity)
        {
            float directionX = collisions.FacingDirection;
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            // Prepare to find the center-most collision point
            bool centralRayHit = false;
            RaycastHit2D centralHitRay = new RaycastHit2D();
            List<RaycastHit2D> hitRays = new List<RaycastHit2D>();

            // Control ray length according to x-velocity compared to skin width
            if (Mathf.Abs(velocity.x) < skinWidth)
                rayLength = 2 * skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? origins.bottomLeft : origins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hitCollider = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                // Debug
                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hitCollider)
                {
                    // Get the slope angle
                    float slopeAngle = Vector2.Angle(hitCollider.normal, Vector2.up);
                    
                    // Use the first ray to check for a slope
                    if(i == 0 && slopeAngle <= maxSlopeAngle)
                    {
                        // Check if transitioning from a descent
                        if (collisions.DescendingSlope)
                        {
                            // If so, use the old velocity for a smoother transition
                            collisions.DescendingSlope = false;
                            velocity = collisions.PrevVelocity;
                        }

                        // Adjust the player to get as close to the slope as possible
                        float distanceToSlopeStart = 0;

                        // Check if climbing a new slope
                        if(slopeAngle != collisions.PrevSlopeAngle)
                        {
                            // Set the distance to the slope
                            distanceToSlopeStart = hitCollider.distance - skinWidth;

                            // Subtract from the velocity so that it only uses
                            // the pure velocity when climbing
                            velocity.x -= distanceToSlopeStart * directionX;
                        }

                        // Climb the slope
                        ClimbSlope(ref velocity, slopeAngle, hitCollider);

                        // Re-add the distance to slope
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    // If not colliding with a slope, use normal collisions
                    if(!collisions.ClimbingSlope || slopeAngle > maxSlopeAngle)
                    {
                        // Adjust the velocity y
                        velocity.x = (hitCollider.distance - skinWidth) * directionX;

                        // Set the ray length equal to the hit distance
                        rayLength = hitCollider.distance;

                        // Update velocity on the y-axis if on a slpe
                        if(collisions.ClimbingSlope)
                        {
                            velocity.y = Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }

                        // Store the hitpoint
                        hitRays.Add(hitCollider);

                        // Check if this is the central ray
                        if (i == centralHorizontalRay)
                        {
                            centralRayHit = true;
                            centralHitRay = hitCollider;
                        }

                        // Set collision info
                        collisions.Right = (directionX == 1);
                        collisions.Left = (directionX == -1);
                        collisions.Layer = layers[hitCollider.transform.gameObject.layer];
                        currentLayer = collisions.Layer;
                    }
                }
            }

            // Check if the central ray was hit
            if(centralRayHit)
            {
                // If so, set it as the collision point
                collisions.HorizontalCollisionRay = centralHitRay;
            } else if(hitRays.Count > 0)
            {
                // Set the max value for distance to compare
                float closestDistance = float.MaxValue;
                float centerY = (origins.bottomLeft.y + origins.topLeft.y) / 2;

                // Iterate through each point and find the closest distance to the center
                foreach (RaycastHit2D ray in hitRays)
                {
                    // Compare the distance
                    float distance = Mathf.Abs(ray.point.y - centerY);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        collisions.HorizontalCollisionRay = ray;
                    }
                }
            }

            // Debug
            if (debug)
                Debug.DrawRay(collisions.HorizontalCollisionRay.point, Vector2.right * directionX, Color.white);
        }

        /// <summary>
        /// Climb a slope
        /// </summary>
        /// <param name="velocity">The current velocity</param>
        /// <param name="slopeAngle">The angle of the slope</param>
        private void ClimbSlope(ref Vector2 velocity, float slopeAngle, RaycastHit2D hit)
        {
            // Use trigonometry to get the adjusted x and y velocities
            float moveDistance = Mathf.Abs(velocity.x);

            float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            if (velocity.y <= climbVelocityY)
            {
                velocity.y = climbVelocityY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                collisions.Below = true;
                collisions.ClimbingSlope = true;
                collisions.LastSlopeVerticalDirection = 1;
                collisions.SlopeAngle = slopeAngle;
                collisions.SlopeNormal = hit.normal;
                collisions.LastSeenSlopeNormal = collisions.SlopeNormal;
                collisions.Layer = layers[hit.transform.gameObject.layer];
                currentLayer = collisions.Layer;
            }
        }

        /// <summary>
        /// Descend a slope
        /// </summary>
        /// <param name="velocity">The current velocity</param>
        public void DescendSlope(ref Vector2 velocity, bool tryingFastSlide = false, PlayerController player = null, float fastSlideScalar = 1f)
        {
            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(origins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(origins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);

            // Check that only one hit is true
            if(maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                // Slide down max slopes
                SlideDownMaxSlope(maxSlopeHitLeft, ref velocity);
                SlideDownMaxSlope(maxSlopeHitRight, ref velocity);
            }

            // Check if we're walking down the slpe
            if(!collisions.SlidingDownMaxSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                Vector2 rayOrigin = (directionX == -1) ? origins.bottomRight : origins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    // Check if on a slope and can descend the slope
                    if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                    {
                        // Check that we're moving down the slope
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            // Check how far down to move based on x-velocity
                            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                            {
                                // Check for fast sliding
                                player.IsFastSliding = (tryingFastSlide && player != null) ? true : false;
                                float moveDistance = tryingFastSlide ? Mathf.Abs(velocity.x) * fastSlideScalar : Mathf.Abs(velocity.x); 
                                float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                                velocity.y -= descendVelocityY;

                                collisions.SlopeAngle = slopeAngle;
                                collisions.DescendingSlope = true;
                                collisions.LastSlopeVerticalDirection = -1;
                                collisions.Below = true;
                                collisions.SlopeNormal = hit.normal;
                                collisions.LastSeenSlopeNormal = collisions.SlopeNormal;
                                collisions.SlopeDescentDirection = (int)Mathf.Sign(hit.normal.x);
                                collisions.Layer = layers[hit.transform.gameObject.layer];
                                currentLayer = collisions.Layer;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Slide down slopes that are greater than or equal to the max slope angle
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="velocity"></param>
        private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 velocity)
        {
            // Check if the raycast hit a slope
            if(hit)
            {
                // Find the angle of the slope
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle > maxSlopeAngle)
                {
                    // Calculate the x-velocity
                    velocity.x = hit.normal.x * (Mathf.Abs(velocity.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    // Set collision info
                    collisions.SlopeAngle = slopeAngle;
                    collisions.SlidingDownMaxSlope = true;
                    collisions.SlopeNormal = hit.normal;
                    collisions.LastSlopeVerticalDirection = -1;
                    collisions.LastSeenSlopeNormal = collisions.SlopeNormal;
                    collisions.Layer = layers[hit.transform.gameObject.layer];
                    currentLayer = collisions.Layer;
                }
            }
        }

        /// <summary>
        /// Set the max slope angle
        /// </summary>
        /// <param name="angle">The angle to set</param>
        public void SetMaxSlopeAngle(float angle) => maxSlopeAngle = angle;
    }
}

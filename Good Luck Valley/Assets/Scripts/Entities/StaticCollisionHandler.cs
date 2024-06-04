using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Entity
{
    public class StaticCollisionHandler : Raycaster
    {
        public struct CollisionInfo
        {
            public bool Above, Below;
            public bool Left, Right;
            public Vector2 PrevVelocity;
            public CollisionLayer Layer;
            public GameObject Entity;

            /// <summary>
            /// Reset the Collision Info
            /// </summary>
            public void ResetInfo()
            {
                Above = Below = false;
                Left = Right = false;
                PrevVelocity = Vector2.zero;
                Layer = CollisionLayer.Ground;
                Entity = null;
            }
        }

        [Header("Fields")]
        [SerializeField] private bool debug;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private CollisionLayer currentLayer;
        [SerializeField] private string collidingEntity;
        public CollisionInfo collisions;
        private Dictionary<int, CollisionLayer> layers;

        // Start is called before the first frame update
        void Start()
        {
            layers = new Dictionary<int, CollisionLayer>()
            {
                {0, CollisionLayer.None },
                {3, CollisionLayer.Ground},
                {6, CollisionLayer.Wall},
                {7, CollisionLayer.MushroomWall},
                {8, CollisionLayer.Slope},
                {9, CollisionLayer.Mushroom },
                {10, CollisionLayer.Player }
            };
        }

        /// <summary>
        /// Handle all collision directions for a static object
        /// </summary>
        /// <param name="velocity"></param>
        public void HandleCollisions(ref Vector2 velocity)
        {
            float rayLengthX = Mathf.Abs(velocity.x) + skinWidth;
            float rayLengthY = Mathf.Abs(velocity.y) + skinWidth;

            // Loop through the left horizontal rays
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = origins.bottomLeft;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayLengthX, collisionMask);

                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.left, Color.red);

                if (hit)
                {
                    // Set the ray length equal to the hit distance
                    rayLengthX = hit.distance;

                    // Set collision info
                    collisions.Left = true;
                    collisions.Layer = layers[hit.transform.gameObject.layer];
                    collisions.Entity = hit.transform.gameObject;
                    currentLayer = collisions.Layer;
                    collidingEntity = collisions.Entity.name;
                } else
                {
                    currentLayer = CollisionLayer.None;
                    collidingEntity = "None";
                }
            }

            // Loop thorugh the right horizontal rays
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = origins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLengthX, collisionMask);

                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.right, Color.red);

                if (hit)
                {
                    // Set the ray length equal to the hit distance
                    rayLengthX = hit.distance;

                    // Set collision info
                    collisions.Right = true;
                    collisions.Layer = layers[hit.transform.gameObject.layer];
                    collisions.Entity = hit.transform.gameObject;
                    currentLayer = collisions.Layer;
                    collidingEntity = collisions.Entity.name;
                } else
                {
                    currentLayer = CollisionLayer.None;
                    collidingEntity = "None";
                }
            }

            // Loop through the upward vertical rays
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = origins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLengthY, collisionMask);

                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.up, Color.red);

                if (hit)
                {
                    // Set the ray length equal to the hit distance
                    rayLengthY = hit.distance;

                    // Set collision info
                    collisions.Above = true;
                    collisions.Layer = layers[hit.transform.gameObject.layer];
                    collisions.Entity = hit.transform.gameObject;
                    currentLayer = collisions.Layer;
                    collidingEntity = collisions.Entity.name;
                } else
                {
                    currentLayer = CollisionLayer.None;
                    collidingEntity = "None";
                }
            }

            // Loop through the downward vertical rays
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = origins.bottomLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLengthY, collisionMask);

                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.down, Color.red);

                if (hit)
                {
                    // Set the ray length equal to the hit distance
                    rayLengthY = hit.distance;

                    // Set collision info
                    collisions.Below = true;
                    collisions.Layer = layers[hit.transform.gameObject.layer];
                    collisions.Entity = hit.transform.gameObject;
                    currentLayer = collisions.Layer;
                    collidingEntity = collisions.Entity.name;
                } else
                {
                    currentLayer = CollisionLayer.None;
                    collidingEntity = "None";
                }
            }
        }
    }
}
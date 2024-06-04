using UnityEngine;

namespace GoodLuckValley.Entities
{
    public class DynamicCircleCollisionHandler : CircleRaycaster
    {
        public struct CollisionInfo
        {
            public bool Collision;

            public void ResetInfo()
            {
                Collision = false;
            }
        }

        [Header("Fields")]
        [SerializeField] private bool debug;
        [SerializeField] private LayerMask collisionMask;
        public CollisionInfo collisions;


        public void UpdateCollisions(ref Vector2 velocity)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for(int i = 0; i < numberOfRays; i++)
            {
                float angle = i * (2 * Mathf.PI / numberOfRays);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 rayOrigin = (Vector2)transform.position + direction * radius;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayLength, collisionMask);

                if (debug)
                    Debug.DrawRay(rayOrigin, direction * rayLength, Color.red);

                if(hit)
                {
                    collisions.Collision = true;
                }
            }
        }
    }
}
using UnityEngine;

namespace GoodLuckValley.Entity
{
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public class Raycaster : MonoBehaviour
    {
        [SerializeField] protected float distanceBetweenRays = 0.10f;
        protected const float skinWidth = 0.015f;
        protected BoxCollider2D entityCollider;
        protected RaycastOrigins origins;
        protected int horizontalRayCount;
        protected int verticalRayCount;
        protected int centralHorizontalRay;
        protected int centralVerticalRay;
        protected float horizontalRaySpacing;
        protected float verticalRaySpacing;

        private void Awake()
        {
            // Get the box collider
            entityCollider = GetComponent<BoxCollider2D>();

            // Calculate the ray spacing
            CalculateRaySpacing();
        }

        /// <summary>
        /// Update the Raycast origins
        /// </summary>
        public void UpdateRaycastOrigins()
        {
            // Get current bounds
            Bounds bounds = entityCollider.bounds;
            bounds.Expand(skinWidth * -2f);

            // Set origin position for raycasts
            origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        /// <summary>
        /// Calculate the spacing between Raycasts
        /// </summary>
        public void CalculateRaySpacing()
        {
            // Get curent collider bounds
            Bounds bounds = entityCollider.bounds;
            bounds.Expand(skinWidth * -2f);

            // Calculate the width and height of the bounds
            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            // Calculate the ray count
            horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

            // Calculate the psacing between all raycasts
            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

            // Set center points
            centralHorizontalRay = horizontalRayCount / 2;
            centralVerticalRay = verticalRayCount / 2;
        }

        public void UpdateCollider()
        {
            UpdateRaycastOrigins();
            CalculateRaySpacing();
        }
    }
}
using GoodLuckValley.Player.Control;
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
        protected const float skinWidth = 0.015f;
        protected BoxCollider2D entityCollider;
        protected RaycastOrigins origins;
        [SerializeField] protected int horizontalRayCount = 4;
        [SerializeField] protected int verticalRayCount = 4;
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
            Bounds bounds = entityCollider.bounds;
            bounds.Expand(skinWidth * -2f);

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
            Bounds bounds = entityCollider.bounds;
            bounds.Expand(skinWidth * -2f);

            // Ensure two rays are set at all times
            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

            // Calculate the psacing between all raycasts
            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }
    }
}
using UnityEngine;

namespace GoodLuckValley.Entities
{
    public class CircleRaycaster : MonoBehaviour
    {
        [SerializeField] protected float distanceBetweenRays = 0.10f; // Desired spacing between rays
        [SerializeField] protected int numberOfRays;
        protected const float skinWidth = 0.015f;
        protected CircleCollider2D entityCollider;
        protected float radius;

        private void Awake()
        {
            entityCollider = GetComponent<CircleCollider2D>();
            radius = entityCollider.radius;
            CalculateRaySpacing();
            UpdateRaycastOrigins();
        }

        /// <summary>
        /// Update the origins of the raycasts
        /// </summary>
        public void UpdateRaycastOrigins()
        {
            Bounds bounds = entityCollider.bounds;
            bounds.Expand(skinWidth * -2f);

            // Adjust the radius with the skin width
            float adjustedRadius = radius - skinWidth;

            for(int i = 0; i < numberOfRays; i++)
            {
                float angle = i * (2 * Mathf.PI / numberOfRays);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 rayOrigin = (Vector2)transform.position + direction * adjustedRadius;

                Debug.DrawRay(rayOrigin, direction * adjustedRadius, Color.red);
            }
        }

        /// <summary>
        /// Calculate the spacing between Raycasts
        /// </summary>
        public void CalculateRaySpacing()
        {
            // Adjust radius with skin width
            float adjustedRadius = radius - skinWidth;
            float circumference = 2 * Mathf.PI * adjustedRadius;
            numberOfRays = Mathf.RoundToInt(circumference / distanceBetweenRays);
        }
    }

}
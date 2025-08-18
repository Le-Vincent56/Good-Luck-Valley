using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class CenterDistanceBasedTrigger : BaseTrigger
    {
        private BoxCollider2D boxCollider;
        
        [Header("Trigger Settings")] 
        [SerializeField] private bool useRadialDistance = false;
        [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool useX = true;
        [SerializeField] private bool useY;
        
        [Header("Fields")]
        [SerializeField] protected float currentIntensity = 0f;
        [SerializeField] private bool playerInTrigger = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Get components
            boxCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the collision object is not the player
            if (!collision.TryGetComponent(out PlayerController player)) return;

            playerInTrigger = true;
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            // Exit if not the player
            if (!collision.TryGetComponent(out PlayerController controller)) return;
            
            // Calculate intensity based on distance from the center
            float intensity = CalculateIntensityFromCenter(controller.transform.position);
            
            // Apply the intensity to effects
            ApplyEffects(intensity);
        }
        
        protected void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collision object is not the player
            if (!collision.TryGetComponent(out PlayerController controller)) return;
            
            playerInTrigger = false;
            
            // Reset to minimum intensity
            ApplyEffects(0f);
        }

        /// <summary>
        /// Calculate the intensity of the effect based on the player's position
        /// to the center
        /// </summary>
        private float CalculateIntensityFromCenter(Vector3 playerPosition)
        {
            // Get the bounds and the center point of the Box Collider
            Bounds bounds = boxCollider.bounds;
            Vector3 center = bounds.center;

            // handle axis isolation
            if (!useX && !useY)
            {
                // If neither axis is selected, default to both
                Debug.LogWarning("No axis selected for distance calculation. Defaulting to both axes.");
                useX = true;
                useY = true;
            }
            
            // Calculate distances for each axis
            float xDistance = useX ? Mathf.Abs(playerPosition.x - center.x) / bounds.extents.x : 0f;
            float yDistance = useY ? Mathf.Abs(playerPosition.y - center.y) / bounds.extents.y : 0f;

            float normalizedDistance = useX switch
            {
                // Apply axis isolation
                true when !useY => xDistance,
                false when useY => yDistance,
                _ => useRadialDistance
                    ? Mathf.Sqrt(xDistance * xDistance + yDistance * yDistance) // Calculate radial distance
                    : Mathf.Max(xDistance, yDistance)
            };
            
            // IMPORTANT: Clamp the normalized distance to ensure it's between 0 and 1
            normalizedDistance = Mathf.Clamp01(normalizedDistance);

            // Invert so the center = 1 and the edges = 0
            float intensity = 1f - normalizedDistance;

            // Apply the intensity curve for additional control
            intensity = intensityCurve.Evaluate(intensity);
            
            // Update debug value
            currentIntensity = intensity;
            
            return intensity;
        }
        
        /// <summary>
        /// Apply effects based on the player's distance to the center
        /// </summary>
        protected virtual void ApplyEffects(float intensity) { /* Noop */ }
    }
}

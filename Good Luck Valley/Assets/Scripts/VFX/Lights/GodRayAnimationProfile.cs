using UnityEngine;

namespace GoodLuckValley.VFX.Lights
{
    [CreateAssetMenu(fileName = "God Ray Profile", menuName = "VFX/God Ray Animation Profile")]
    public class GodRayAnimationProfile : ScriptableObject
    {
        [Header("Intensity Animation")] 
        [Range(0f, 1f)] public float minIntensityMultiplier = 0.5f;
        [Range(0f, 1f)] public float maxIntensityMultiplier = 1f;
        [Range(0.01f, 2f)] public float animationSpeed = 0.3f;

        [Header("Perlin Noise Settings")] 
        [Tooltip("Scale of the Perlin Noise. Higher = more rapid changes")]
        [Range(0.1f, 5f)] public float noiseScale = 1f;
        public bool useRandomOffset = true;

        [Header("Advanced Settings")]
        [Tooltip("Smoothing factor for transitions. Higher = smoother")]
        [Range(0.01f, 1f)] public float smoothingFactor = 0.1f;
        [Tooltip("Animation curve to shape the intensity over the noise value")]
        public AnimationCurve intensityCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private void OnValidate()
        {
            // Clamp the ranges
            minIntensityMultiplier = Mathf.Clamp01(minIntensityMultiplier);
            maxIntensityMultiplier = Mathf.Clamp01(maxIntensityMultiplier);
            
            // Exit case - the min is less than or equal to the max
            if (minIntensityMultiplier <= maxIntensityMultiplier) return;
            
            // Ensure the min is less than the max
            (minIntensityMultiplier, maxIntensityMultiplier) = (maxIntensityMultiplier, minIntensityMultiplier);
        }
    }
}
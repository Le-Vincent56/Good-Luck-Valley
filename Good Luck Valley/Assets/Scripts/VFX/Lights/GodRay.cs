using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX.Lights
{
    [RequireComponent(typeof(Light2D))]
    public class GodRay : MonoBehaviour, ILightAnimator
    {
        [Header("Configuration")] 
        [SerializeField] private GodRayAnimationProfile profile;

        [Header("Runtime Control")] 
        [SerializeField] private bool animationEnabled = true;

        [Header("Debug")] 
        [SerializeField] private bool showDebugInfo = false;
        
        private Light2D targetLight;

        [SerializeField] private float baseIntensity;
        [SerializeField] private float currentIntensity;
        private float noiseOffset;
        private float timeOffset;

        [SerializeField] private float cachedNoiseValue;
        private float lastUpdateTime;
        private const float UPDATE_FREQUENCY = 0.016f; // ~60 FPS update rate

        private void Awake()
        {
            Initialize(GetComponent<Light2D>());
        }

        private void OnEnable()
        {
            // Exit case - no target light exists yet
            if (!targetLight) return;

            // Enable the animation
            animationEnabled = true;
        }

        private void OnDisable()
        {
            // Reset to the base values
            ResetToBaseValues();
        }

        private void Update()
        {
            // Exit case - the animation isn't enabled, or not
            // profile is set
            if (!animationEnabled || !profile) return;
            
            // Update the animation
            UpdateAnimation(Time.deltaTime);
        }

        private void OnValidate()
        {
            // Exit cases
            if (!profile) return;
            if (!targetLight) return;
            if (Application.isPlaying) return;
            
            targetLight.intensity = baseIntensity;
        }

        /// <summary>
        /// Initialize the god ray
        /// </summary>
        public void Initialize(Light2D targetLight)
        {
            this.targetLight = targetLight;

            // Exit case - the target light doesn't exists
            if (!targetLight) return;
            
            // Set the intensities
            baseIntensity = targetLight.intensity;
            currentIntensity = baseIntensity;
            
            // Exit case - if there's no profile set or not randomizing the offset
            if (!profile || !profile.useRandomOffset) return;

            noiseOffset = Random.Range(0f, 1000f);
            timeOffset = Random.Range(0f, 100f);
        }

        /// <summary>
        /// Update the god ray's intensity using Perlin Noise
        /// </summary>
        public void UpdateAnimation(float deltaTime)
        {
            // Exit case - there is no target light or profile set
            if (!targetLight || !profile) return;

            float currentTime = Time.time;

            // Limit the update frequency for performance
            if (currentTime - lastUpdateTime < UPDATE_FREQUENCY) return;
            
            // Update the last animation time
            lastUpdateTime = currentTime;
            
            // Calculate Perlin noise value
            float noiseX = (currentTime + timeOffset) * profile.animationSpeed;
            float noiseY = noiseOffset;
            float rawNoise = Mathf.PerlinNoise(noiseX * profile.noiseScale, noiseY);
            
            // Apply the animation curve
            float shapedNoise = profile.intensityCurve.Evaluate(rawNoise);
            
            // Map the noise to the intensity range
            float targetIntensityMultiplier = Mathf.Lerp(
                profile.minIntensityMultiplier,
                profile.maxIntensityMultiplier,
                shapedNoise
            );
            
            // Apply smoothing
            float smoothedMultiplier = Mathf.Lerp(
                currentIntensity / baseIntensity,
                targetIntensityMultiplier,
                profile.smoothingFactor
            );
            
            // Apply to the light
            currentIntensity = baseIntensity * smoothedMultiplier;
            targetLight.intensity = currentIntensity;
            
            // Cache for debug
            cachedNoiseValue = shapedNoise;
        }

        /// <summary>
        /// Set the god ray to be enabled
        /// </summary>
        /// <param name="enabled"></param>
        public void SetEnabled(bool enabled)
        {
            animationEnabled = enabled;

            // Exit case - the animation is enabled
            if (enabled) return;
            
            ResetToBaseValues();
        }

        /// <summary>
        /// Reset the god ray's values
        /// </summary>
        public void ResetToBaseValues()
        {
            // Exit case - there is no target light
            if (!targetLight) return;

            targetLight.intensity = baseIntensity;
            currentIntensity = baseIntensity;
        }
    }
}

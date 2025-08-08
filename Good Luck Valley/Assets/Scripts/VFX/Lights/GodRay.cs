using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX.Lights
{
    [RequireComponent(typeof(Light2D))]
    public class GodRay : MonoBehaviour, ILightAnimator
    {
        [Header("References")] 
        [SerializeField] private GodRayAnimationProfile profile;
        private Light2D targetLight;

        [Header("Fields")]
        [SerializeField] private bool animationEnabled = true;
        [SerializeField] private float baseIntensity;
        [SerializeField] private float currentIntensity;
        private float noiseOffset;
        private float timeOffset;
        private float lastUpdateTime;
        private const float UPDATE_FREQUENCY = 0.016f; // ~60 FPS update rate

        [Header("Tweening Fields")] 
        [SerializeField] private bool usesTweening = false;
        [SerializeField] private float tweenDuration;
        [SerializeField] private bool filterOnFinished = true;
        [SerializeField] private bool runningTween = false;
        private float finalValue;
        private Tween intensityTween;

        private void Awake()
        {
            Initialize(GetComponent<Light2D>());
        }

        private void OnDisable()
        {
            // Reset to the base values
            ResetToBaseValues();

            // Kill any existing tweens
            intensityTween?.Kill();
        }

        private void Update()
        {
            // Exit case - the animation isn't enabled, or not
            // profile is set
            if (!animationEnabled || !profile) return;
            
            // Exit case - if filtering on finished tween but the
            // tween hasn't finished yet
            if (usesTweening && filterOnFinished && runningTween) return;
            
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

            if (usesTweening)
            {
                targetLight.intensity = 0f;
                finalValue = baseIntensity;
            }

            runningTween = false;
            
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

        /// <summary>
        /// Turn the god ray on
        /// </summary>
        public void On()
        {
            // Kill the intensity tween if it exists already
            intensityTween?.Kill();
            
            // Set the light to the flash value
            intensityTween = DOTween.To(
                () => targetLight.intensity,
                x => targetLight.intensity = x,
                finalValue,
                tweenDuration
            ).SetEase(Ease.InOutSine);

            intensityTween.OnStart(() => runningTween = true);
            intensityTween.OnComplete(() =>
            {
                runningTween = false;
                animationEnabled = true;
            });
        }
    }
}

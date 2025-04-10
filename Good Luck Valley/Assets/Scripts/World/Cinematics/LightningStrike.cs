using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.World;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.World.Cinematics
{
    public class LightningStrike : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light2D globalLight;

        [Header("Tweening Variables")]
        [SerializeField] private float lightningIntensity;
        [SerializeField] private float strikeDuration;
        private Tween lightningTween;

        private EventBinding<StrikeLightning> onStrikeLightning;

        private void Awake()
        {
            // Get components
            globalLight = GetComponent<Light2D>();
        }

        private void OnEnable()
        {
            onStrikeLightning = new EventBinding<StrikeLightning>(InvokeLightning);
            EventBus<StrikeLightning>.Register(onStrikeLightning);
        }

        private void OnDisable()
        {
            EventBus<StrikeLightning>.Deregister(onStrikeLightning);
        }

        private void OnDestroy()
        {
            // Kill any existing tweens
            lightningTween?.Kill();
        }

        /// <summary>
        /// Invoke the lightning to flash the screen
        /// </summary>
        private void InvokeLightning()
        {
            // Set the halved duration
            float halvedDuration = strikeDuration / 2f;

            // Set the light to the flash value
            Intensify(30f, halvedDuration / 2f, () =>
            {
                // Set the light back to 1
                Intensify(1f, halvedDuration / 2f);
            });
        }
        
        /// <summary>
        /// Tween the global light intensity
        /// </summary>
        private void Intensify(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Lightning Tween if it exists
            lightningTween?.Kill();

            // Set the light to the flash value
            lightningTween = DOTween.To(
                () => globalLight.intensity,
                    x => globalLight.intensity = x,
                    endValue,
                    duration
            );

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up the completion action
            lightningTween.onComplete += onComplete;
        }
    }
}

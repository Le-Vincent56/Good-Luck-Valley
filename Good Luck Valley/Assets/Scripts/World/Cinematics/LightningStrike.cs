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
        [SerializeField] private Color lightningColor;

        private Sequence lightningSequence;

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
            // Kill any existing tweens or sequences
            lightningSequence?.Kill();
        }

        /// <summary>
        /// Invoke the lightning to flash the screen
        /// </summary>
        private void InvokeLightning(StrikeLightning eventData)
        {
            // Kill the Lightning Sequence if it exists
            lightningSequence?.Kill();

            // Create a new sequence
            lightningSequence = DOTween.Sequence();

            // Calculate the halved duration
            float halvedDuration = eventData.Duration / 2f;

            // Set the beginning of the strike
            lightningSequence.Append(IntensifyLight(eventData.Intensity, halvedDuration));
            lightningSequence.Join(ChangeColor(lightningColor, halvedDuration));

            // Set the ending of the strike
            lightningSequence.Append(IntensifyLight(0f, halvedDuration));
            lightningSequence.Join(ChangeColor(Color.white, halvedDuration));

            // Play the sequence
            lightningSequence.Play();
        }
        
        /// <summary>
        /// Tween the global light intensity
        /// </summary>
        private Tween IntensifyLight(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Set the light to the flash value
            Tween flashTween = DOTween.To(
                () => globalLight.intensity,
                    x => globalLight.intensity = x,
                    endValue,
                    duration
            );

            // Exit case - no completion action was given
            if (onComplete == null) return flashTween;

            // Hook up the completion action
            flashTween.onComplete += onComplete;

            return flashTween;
        }

        private Tween ChangeColor(Color endValue, float duration, TweenCallback onComplete = null)
        {
            // Set the color of the light
            Tween colorTween = DOTween.To(
                () => globalLight.color,
                    x => globalLight.color = x,
                    endValue,
                    duration
            );

            // Exit case - no completion action was given
            if (onComplete == null) return colorTween;

            // Hook up the completion action
            colorTween.onComplete += onComplete;

            return colorTween;
        }
    }
}

using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Fireflies;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Lantern : GateInteractable
    {
        [Header("Lantern")]
        [SerializeField] private int channel;
        [SerializeField] private Light2D activateLight;
        [SerializeField] private ParticleSystem[] lanternParticles;

        [Header("Tweening Variables")]
        [SerializeField] private float toOuterRadius;
        [SerializeField] private float lightOutRatio;
        private float initialOuterRadius;
        private float illuminateDuration;
        private Tween illuminateTween;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new LanternStrategy(this);

            // Get components
            activateLight = GetComponentInChildren<Light2D>();
            lanternParticles = GetComponentsInChildren<ParticleSystem>();

            // Set variables
            illuminateDuration = lanternParticles[0].main.duration;
            initialOuterRadius = activateLight.pointLightOuterRadius;
        }

        protected override void OnDestroy()
        {
            // Call the parent OnDestroy()
            base.OnDestroy();

            // Kill the Illuminate Tween if it exists
            illuminateTween?.Kill();
        }

        public override void Interact()
        {
            // Exit case - the Handler has no value
            if (!handler.HasValue) return;

            // Exit case - the Interactable cannot be interaacted with
            if (!canInteract) return;

            // Exit case - the Interactable Strategy fails
            if (!strategy.Interact(handler.Value)) return;

            // Remove the Interactable from the Handler
            handler.Match(
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Set un-interactable
            canInteract = false;

            // Fade out the sprites and deactivate
            FadeFeedback(0f, fadeDuration);
        }

        /// <summary>
        /// Activate the effects of the Lantern
        /// </summary>
        public void Activate()
        {
            // Raise the Activate Lantern event
            EventBus<ActivateLantern>.Raise(new ActivateLantern()
            {
                Channel = channel
            });

            // Iterate through each Particle System
            foreach(ParticleSystem lanternParticle in lanternParticles)
            {
                // Play the Particle System
                lanternParticle.Play();
            }

            // Increase the outer light radius to the far value
            Illuminate(toOuterRadius, illuminateDuration * lightOutRatio, Ease.OutQuad, () =>
            {
                // Wait for a second
                Illuminate(toOuterRadius, 1f, Ease.OutQuad, () =>
                {
                    // Decrease the outer light radius back to the initial value
                    Illuminate(initialOuterRadius, illuminateDuration * 2f, Ease.OutQuad);
                });
            });
        }

        /// <summary>
        /// Check if the Player has Fireflies for the Lantern
        /// </summary>
        protected override bool CheckForKey(InteractableHandler handler)
        {
            // Set to false by default
            bool hasKey = false;

            // Check if the Firefly Handler has a fruit
            handler.FireflyHandler.GetFireflies().Match(
                onValue: fireflies =>
                {
                    // If the player has Fireflies, they have the key
                    hasKey = true;

                    return 0;
                },
                onNoValue: () =>
                {
                    // The Firefly Handler does not have a Fruit, so do nothing
                    return 0;
                }
            );

            return hasKey;
        }

        /// <summary>
        /// Handle light illumination tweening for the Lantern
        /// </summary>
        private void Illuminate(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Illuminate Tween if it exists
            illuminateTween?.Kill();

            // Tween the m_TrackedObjectOffset to the endValue using the duration and set an Ease
            illuminateTween = DOTween.To(
                () => activateLight.pointLightOuterRadius,
                    x => activateLight.pointLightOuterRadius = x,
                    endValue,
                    duration
            );

            // Set the Ease type
            illuminateTween.SetEase(easeType);

            // Exit case - if there is no completion actino
            if (onComplete == null) return;

            // Hook up the completion action
            illuminateTween.onComplete += onComplete;
        }
    }
}

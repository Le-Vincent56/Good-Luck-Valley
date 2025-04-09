using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Fireflies;
using GoodLuckValley.Events.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Lantern : GateInteractable
    {
        [Header("Lantern")]
        [SerializeField] private int channel;
        [SerializeField] private Light2D onLight;
        [SerializeField] private Light2D playerLight;
        [SerializeField] private List<ParticleSystem> lanternParticles;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Sprite activeSprite;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event lanternPlaceEvent;

        [Header("Tweening Variables")]
        [SerializeField] private float toFlash;
        [SerializeField] private float toOuterRadius;
        [SerializeField] private float lightOutRatio;
        [SerializeField] private float flashOutDuration;
        private float illuminateDuration;
        private float playerLightIntensity;
        private Tween illuminateTween;
        private Tween flashTween;
        private Tween playerLightTween;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new LanternStrategy(this);

            // Get components
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            onLight = lights[0];
            playerLight = lights[1];

            GetComponentsInChildren(lanternParticles);

            // Set variables
            illuminateDuration = lanternParticles[0].main.duration;
            playerLightIntensity = playerLight.intensity;
            playerLight.intensity = 0f;
        }

        protected override void OnDestroy()
        {
            // Call the parent OnDestroy()
            base.OnDestroy();

            // Kill existing Tweens
            illuminateTween?.Kill();
            flashTween?.Kill();
            playerLightTween?.Kill();
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

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = fadeDuration
            });
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

            // Swap the sprite
            interactableSprite.sprite = activeSprite;

            // Pay the Lantern Place SFX
            lanternPlaceEvent.Post(gameObject);

            // Increase the outer light radius to the far value
            Illuminate(toOuterRadius, illuminateDuration * lightOutRatio, Ease.OutQuad);

            // Increase the lantern's intensity for the environment
            Intensify(flashTween, onLight, toFlash, flashOutDuration, Ease.InOutSine);

            // Increase the lantern's intensity for the player
            Intensify(playerLightTween, playerLight, playerLightIntensity, flashOutDuration, Ease.InOutSine);
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
        private void Illuminate(float endValue, float duration, Ease easeType)
        {
            // Kill the Illuminate Tween if it exists
            illuminateTween?.Kill();

            // Tween the outer radius to the endValue using the duration and set an Ease
            illuminateTween = DOTween.To(
                () => onLight.pointLightOuterRadius,
                    x => onLight.pointLightOuterRadius = x,
                    endValue,
                    duration
            );

            // Set the Ease type
            illuminateTween.SetEase(easeType);
        }

        private void Intensify(Tween tween, Light2D light, float endValue, float duration, Ease easeType)
        {
            // Kill the Flash Tween if it exists
            tween?.Kill();

            // Tween the intensity to the endValue using the duration and set an Ease
            tween = DOTween.To(
                () => light.intensity,
                    x => light.intensity = x,
                    endValue,
                    duration
            );

            // Set the Ease type
            tween.SetEase(easeType);
        }
    }
}

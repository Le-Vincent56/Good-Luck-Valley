using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Fruit : Collectible
    {
        private Light2D backLight;
        private Light2D fruitLight;
        private ParticleSystem ambientParticles;
        private ParticleSystem collectParticles;

        [Header("Tweening Variables")]
        [SerializeField] private float flashValue;
        [SerializeField] private float flashDuration;
        [SerializeField] private float withdrawIntensityDuration;
        [SerializeField] private float sendIntensityDuration;
        private float toIntensity;
        private Sequence intensitySequence;
        private Sequence flashSequence;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new FruitStrategy(this);

            // Get components
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            backLight = lights[0];
            fruitLight = lights[1];

            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            ambientParticles = particles[0];
            collectParticles = particles[1];

            // Set variables
            toIntensity = backLight.intensity;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            intensitySequence?.Kill();
            flashSequence?.Kill();
        }

        /// <summary>
        /// Use the Fruit
        /// </summary>
        public void Use(InteractableHandler handler)
        {
            // Remove the Firefly Handler's Fruit
            handler.FireflyHandler.SetFruit(Optional<Fruit>.NoValue);

            // Fade in the Fruit and allow interaction
            //FadeInteractable(1f, fadeDuration, () => canInteract = true);

            //// Tween the light back in
            //Illuminate(toIntensity, sendIntensityDuration, Ease.OutQuad);

            //// Play the particles
            //ambientParticles.Play();
        }

        protected override void Collect()
        {
            // Flash the light
            Flash(flashValue, flashDuration, () =>
            {
                // Remove the light
                Illuminate(0f, withdrawIntensityDuration, Ease.OutQuad);
            });

            // Stop the particles
            ambientParticles.Stop();

            // Play the collection particles
            collectParticles.Play();
        }

        /// <summary>
        /// Handle light illumination tweening for the Lantern
        /// </summary>
        private void Illuminate(float intensity, float intensityDuration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Illuminate Tween if it exists
            intensitySequence?.Kill();

            // Tween the back light
            Tween backLightTween = DOTween.To(
                () => backLight.intensity,
                    x => backLight.intensity = x,
                    intensity,
                    intensityDuration
            ).SetEase(easeType);

            // Tween the fruit light
            Tween fruitLightTween = DOTween.To(
                () => fruitLight.intensity,
                    x => fruitLight.intensity = x,
                    intensity,
                    intensityDuration
            ).SetEase(easeType);

            // Join the sequences together
            intensitySequence.Append(backLightTween);
            intensitySequence.Join(fruitLightTween);

            // Exit case - if there is no completion actino
            if (onComplete == null) return;

            // Hook up the completion action
            intensitySequence.onComplete += onComplete;
        }

        private void Flash(float flashValue, float flashDuration, TweenCallback onComplete = null)
        {
            // Kill the Flash Sequence if it exists
            flashSequence?.Kill();

            // Create the Flash Sequence
            flashSequence = DOTween.Sequence();

            // Set the light to the flash value
            Tween outTween = DOTween.To(
                () => backLight.intensity,
                    x => backLight.intensity = x,
                    flashValue,
                    flashDuration / 2f
            ).SetEase(Ease.OutQuad);

            // Set the light to 0
            Tween inTween = DOTween.To(
                () => backLight.intensity,
                    x => backLight.intensity = x,
                    0f,
                    flashDuration / 2f
            ).SetEase(Ease.InOutSine);

            // Append the outTween and inTween to the sequence
            flashSequence.Append(outTween);
            flashSequence.Append(inTween);

            // Exit case - if there is no completion actino
            if (onComplete == null) return;

            // Hook up the completion action
            flashSequence.onComplete += onComplete;
        }
    }
}

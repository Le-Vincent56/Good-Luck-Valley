using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.Interactables.Journal
{
    public class JournalPickup : Collectible
    {
        private ParticleSystem particles;
        private Light2D journalLight;

        [Header("Tweening Variables")]
        [SerializeField] private float floatAmount;
        [SerializeField] private float floatSpeed;
        private Tween floatTween;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new JournalPickupStrategy();

            // Get the particle system
            particles = GetComponentInChildren<ParticleSystem>();
            journalLight = GetComponentInChildren<Light2D>();
        }

        protected override void OnDestroy()
        {
            // Call the parent OnDestroy()
            base.OnDestroy();

            // Kill the Float Tween if it exists
            floatTween?.Kill();
        }

        protected void OnDisable()
        {
            // Kill the Float Tween if it exists
            floatTween?.Kill();
        }

        private void Start()
        {
            // Exit case - the Journal has been collected
            if (collected) return;

            // Play the particles
            particles.Play();

            // Start the Float
            Float();
        }

        protected override void Collect()
        {
            // Stop the Particle System
            particles.Stop();
            journalLight.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handle floating for the Journal
        /// </summary>
        private void Float()
        {
            // Kill the Float Tween if it exists
            floatTween?.Kill();

            // Set the Float Tween
            floatTween = transform.DOLocalMoveY(transform.localPosition.y + floatAmount, floatSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}

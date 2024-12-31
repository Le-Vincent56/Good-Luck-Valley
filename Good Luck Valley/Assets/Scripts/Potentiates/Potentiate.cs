using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace GoodLuckValley.Potentiates
{
    public enum PotentiateType
    {
        TimeWarp,
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Potentiate : MonoBehaviour
    {
        private SpriteRenderer sprite;
        [SerializeField] private PotentiateType type;
        [SerializeField] private float potentiateDuration;
        private PotentiateStrategy strategy;
        private bool canPotentiate;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private void Awake()
        {
            // Get components
            sprite = GetComponent<SpriteRenderer>();

            // Set variables
            canPotentiate = true;

            // Initialize the Potentiate strategy
            switch (type)
            {
                case PotentiateType.TimeWarp:
                    strategy = new TimeWarpStrategy(this, potentiateDuration);
                    break;
            }
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the Potentiate cannot be used
            if (!canPotentiate) return;

            // Exit case - there is no PlayerController on the collision object
            if (!collision.TryGetComponent(out PotentiateHandler handler)) return;

            // Exit case - the Potentiate strategy failed
            if (!strategy.Potentiate(handler.Controller, handler)) return;

            // Set this as the last Potentiate
            handler.SetLastPotentiate(this);

            // Set to not Potentiate
            canPotentiate = false;

            // Fade out the Potentiate
            Fade(0f);
        }

        /// <summary>
        /// Handle fade tweening for the Potentiate sprite
        /// </summary>
        public void Fade(float endValue, TweenCallback onComplete = null)
        {
            // Kill the Fade tween
            fadeTween?.Kill();

            // Set the Fade tween
            fadeTween = sprite.DOFade(endValue, fadeDuration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion action
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Deplete the strategy
        /// </summary>
        public void Deplete() => strategy.Deplete();

        /// <summary>
        /// Allow the Potentiate to be used
        /// </summary>
        public void AllowPotentiation() => canPotentiate = true;
    }
}

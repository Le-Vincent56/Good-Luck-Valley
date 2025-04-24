using DG.Tweening;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class SeeThroughTrigger : EnterExitTrigger
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float targetOpacity;
        private Material material;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Get the material
            material = spriteRenderer.material;
        }

        private void OnDestroy()
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();
        }

        public override void OnEnter(PlayerController controller)
        {
            // Set the material fade opacity to the target opacity
            Fade(targetOpacity, fadeDuration);
        }

        public override void OnExit(PlayerController controller)
        {
            // Set the material fade opacity to full opacity
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Handle fading the material opacity
        /// </summary>
        private void Fade(float endValue,  float duration)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Create a new fade tween
            fadeTween = DOTween.To(() => material.GetFloat("_FadeOpacity"), x => material.SetFloat("_FadeOpacity", x), endValue, duration);
        }
    }
}

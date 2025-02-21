using DG.Tweening;
using UnityEngine;

namespace GoodLuckValley.World.Revealables
{
    public class RevealableSprite : Revealable
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            // Get the SpriteRenderer
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Fade out and disable the Sprite Renderer
            spriteRenderer.DOFade(0f, 0f);
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        protected override void Reveal()
        {
            // Enable the SpriteRenderer
            spriteRenderer.enabled = true;

            // Fade in the Revealable
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Handle Tweening for the SpriteRenderer's opacity
        /// </summary>
        private void Fade(float endValue, float fadeDuration)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Tween the alpha channel of the SpriteRenderer's color
            fadeTween = spriteRenderer.DOFade(endValue, fadeDuration);
        }
    }
}

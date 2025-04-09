using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GoodLuckValley.World.Revealables
{
    public class RevealableSprite : Revealable
    {
        private TilemapCollider2D tilemapCollider;
        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Get the SpriteRenderer
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            tilemapCollider = GetComponent<TilemapCollider2D>();

            // Fade out and disable the Sprite Renderer
            Fade(0f, 0f);
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        protected override void Reveal()
        {
            // Enable the SpriteRenderer
            spriteRenderer.enabled = true;

            // Enable the tilemap
            tilemapCollider.enabled = true;

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

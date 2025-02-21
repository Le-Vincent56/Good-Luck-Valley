using DG.Tweening;
using UnityEngine;

namespace GoodLuckValley.World.Revealables
{
    public class RevealableSpriteHolder : Revealable
    {
        private SpriteRenderer[] spriteRenderers;
        private Sequence fadeSequence;

        private void Awake()
        {
            // Get the SpriteRenderer
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Iterate over each Sprite Renderer
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                // Disable the Sprite Renderer
                spriteRenderer.enabled = false;
            }

            // Fade out the SpriteRenderers
            Fade(0f, 0f);
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        protected override void Reveal()
        {
            // Iterate over each Sprite Renderer
            foreach(SpriteRenderer spriteRenderer in spriteRenderers)
            {
                // Enable the Sprite Renderer
                spriteRenderer.enabled = true;
            }

            // Fade in the Revealable
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Handle Tweening for the SpriteRenderer's opacity
        /// </summary>
        private void Fade(float endValue, float fadeDuration)
        {
            // Kill the Fade Sequence if it exists
            fadeSequence?.Kill();

            // Create a new Fade Sequence
            fadeSequence = DOTween.Sequence();

            // Iterate through each Sprite Renderer
            foreach(SpriteRenderer spriteRenderer in spriteRenderers)
            {
                // Fade the alpha channel of the SpriteRenderer's color
                fadeSequence.Join(spriteRenderer.DOFade(endValue, fadeDuration));
            }

            // Play the Fade Sequence
            fadeSequence.Play();
        }
    }
}

using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Fireflies;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GoodLuckValley.World.Revealables
{
    public class RevealableTiles : Revealable
    {
        private Tilemap tilemap;
        private TilemapCollider2D tilemapCollider;
        private Color originalColor;

        private void Awake()
        {
            // Get the tilemap
            tilemap = GetComponent<Tilemap>();
            tilemapCollider = GetComponent<TilemapCollider2D>();

            // Store the original color of the tilemap
            originalColor = tilemap.color;

            // Fade out and disable the tilemap
            Fade(0f, 0f);
            tilemapCollider.enabled = false;
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        protected override void Reveal()
        {
            // Enable the tilemap
            tilemapCollider.enabled = true;

            // Fade in the Revealable
            //Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Handle Tweening for the Tilemap's opacity
        /// </summary>
        private void Fade(float endValue, float fadeDuration)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Tween the alpha channel of the Tilemap's color
            fadeTween = DOTween.To(
                () => tilemap.color,
                x => tilemap.color = x,
                new Color(originalColor.r, originalColor.g, originalColor.b, endValue),
                fadeDuration
            );
        }
    }
}

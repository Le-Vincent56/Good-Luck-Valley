using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class AssetCleanserTrigger : BaseTrigger
    {
        [Header("References")]
        [SerializeField] private List<SpriteRenderer> assetsToCleanse = new List<SpriteRenderer>();

        private bool inTrigger = false;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Sequence fadeSequence;

        private void OnDestroy()
        {
            // Kill the Fade Sequence if it exists
            fadeSequence?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Fade out
            Fade(0f);

            // Set inside trigger
            inTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - the player is already inside the trigger
            if (inTrigger) return;

            // Fade out
            Fade(0f);

            // Set inside trigger
            inTrigger = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Fade in
            Fade(1f);

            // Set outside trigger
            inTrigger = false;
        }

        /// <summary>
        /// Handle the fading
        /// </summary>
        /// <param name="endValue"></param>
        private void Fade(float endValue)
        {
            // Kill the Fade Sequence if it exists
            fadeSequence?.Kill();

            // Create the Fade Sequence
            fadeSequence = DOTween.Sequence();

            // Iterate throguh each Sprite Renderer to cleanse
            foreach (SpriteRenderer asset in assetsToCleanse)
            {
                // Add the Fade to the Sequence
                fadeSequence.Join(asset.DOFade(endValue, fadeDuration));
            }

            // Play the fade sequence
            fadeSequence.Play();
        }
    }
}

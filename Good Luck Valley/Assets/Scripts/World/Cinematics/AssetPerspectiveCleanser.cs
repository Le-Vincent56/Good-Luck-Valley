using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class AssetPerspectiveCleanser : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SpriteRenderer> assetsToCleanse = new List<SpriteRenderer>();

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Sequence fadeSequence;

        private void OnDestroy()
        {
            // Kill the Fade Sequence if it exists
            fadeSequence?.Kill();
        }

        /// <summary>
        /// Cleanse the assets by fading them out
        /// </summary>
        public void CleanseAssets() => Fade(0f);

        private void Fade(float endValue)
        {
            // Kill the Fade Sequence if it exists
            fadeSequence?.Kill();

            // Create the Fade Sequence
            fadeSequence = DOTween.Sequence();

            // Iterate throguh each Sprite Renderer to cleanse
            foreach(SpriteRenderer asset in assetsToCleanse)
            {
                // Add the Fade to the Sequence
                fadeSequence.Join(asset.DOFade(endValue, fadeDuration));
            }

            // Play the fade sequence
            fadeSequence.Play();
        }
    }
}

using System.Collections;
using UnityEngine;

namespace GoodLuckValley.UI.Cinematics
{
    public class CinematicBar : MonoBehaviour
    {
        private enum Type
        {
            Top,
            Bottom
        }

        [SerializeField] private Type type;
        private RectTransform rectTransform;

        private Vector2 hiddenAnchorMin;
        private Vector2 hiddenAnchorMax;
        private Vector2 shownAnchorMin;
        private Vector2 shownAnchorMax;

        private Coroutine showCoroutine;
        private Coroutine hideCoroutine;

        private void Awake()
        {
            // Get components
            rectTransform = GetComponent<RectTransform>();

            // Set anchor positions
            switch (type)
            {
                case Type.Top:
                    hiddenAnchorMin = new Vector2(0f, 1f);
                    hiddenAnchorMax = new Vector2(1f, 1f);
                    shownAnchorMin = new Vector2(0f, 0.9f);
                    shownAnchorMax = new Vector2(1f, 1f);
                    break;
                case Type.Bottom:
                    hiddenAnchorMin = new Vector2(0f, 0f);
                    hiddenAnchorMax = new Vector2(1f, 0f);
                    shownAnchorMin = new Vector2(0f, 0f);
                    shownAnchorMax = new Vector2(1f, 0.1f);
                    break;
            }

            // Hide the bars
            rectTransform.anchorMin = hiddenAnchorMin;
            rectTransform.anchorMax = hiddenAnchorMax;
        }

        /// <summary>
        /// Show the Cinematic Bar
        /// </summary>
        public void Show(float duration)
        {
            // Stop any ongoing coroutines
            StopCoroutines();
            showCoroutine = StartCoroutine(AnimateAnchors(shownAnchorMin, shownAnchorMax, duration));
        }

        /// <summary>
        /// Hide the Cinematic Bar
        /// </summary>
        public void Hide(float duration)
        {
            // Stop any ongoing coroutines
            StopCoroutines();
            hideCoroutine = StartCoroutine(AnimateAnchors(hiddenAnchorMin, hiddenAnchorMax, duration));
        }

        /// <summary>
        /// Stop all Cinematic Bar coroutines
        /// </summary>
        private void StopCoroutines()
        {
            // Check if the show coroutine is not null
            if (showCoroutine != null)
                // If so, stop it
                StopCoroutine(showCoroutine);

            // Check if the hide coroutine is not null
            if (hideCoroutine != null)
                // If so, stop it
                StopCoroutine(hideCoroutine);
        }

        /// <summary>
        /// Coroutine to animate the anchor points
        /// </summary>
        private IEnumerator AnimateAnchors(Vector2 targetAnchorMin, Vector2 targetAnchorMax, float duration)
        {
            Vector2 startAnchorMin = rectTransform.anchorMin;
            Vector2 startAnchorMax = rectTransform.anchorMax;
            float elapsedTime = 0f;

            while(elapsedTime < duration)
            {
                // Get the t value
                float t = elapsedTime / duration;

                // Set the anchor points
                rectTransform.anchorMin = Vector2.Lerp(startAnchorMin, targetAnchorMin, t);
                rectTransform.anchorMax = Vector2.Lerp(startAnchorMax, targetAnchorMax, t);
                
                // Increment the elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Finalize the anchor points
            rectTransform.anchorMin = targetAnchorMin;
            rectTransform.anchorMax = targetAnchorMax;
        }
    }
}
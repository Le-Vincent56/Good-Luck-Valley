using DG.Tweening;
using UnityEngine;

namespace GoodLuckValley.UI
{
    public class CanvasGroupBlinker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup groupToBlink;

        [Header("Tweening Variables")]
        [SerializeField] private float blinkDuration;
        private float startingAlpha;
        private Tween fadeTween;

        private void Awake()
        {
            // Get components
            groupToBlink = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            // Set the starting color to invisible
            startingAlpha = 0f;
        }

        private void OnDestroy()
        {
            // Kill the fade Tween if it exists
            fadeTween?.Kill();
        }

        /// <summary>
        /// Start the blinking effect
        /// </summary>
        public void StartBlink()
        {
            Fade(1f, blinkDuration, () =>
            {
                Fade(startingAlpha, blinkDuration, StartBlink);
            });
        }

        /// <summary>
        /// Stop the blinking effect
        /// </summary>
        public void Stop() =>  fadeTween?.Kill();

        /// <summary>
        /// Handle fading for the blinking effect
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = groupToBlink.DOFade(endValue, duration);

            // Exit case - if there is no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}

using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using UnityEngine;

namespace GoodLuckValley.UI.Tutorial
{
    public class CanvasGroupFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fields")]
        [SerializeField] private int id;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<FadeCanvasGroup> onFadeCanvasGroup;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            onFadeCanvasGroup = new EventBinding<FadeCanvasGroup>(HandleFading);
            EventBus<FadeCanvasGroup>.Register(onFadeCanvasGroup);
        }

        private void OnDisable()
        {
            EventBus<FadeCanvasGroup>.Deregister(onFadeCanvasGroup);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        private void HandleFading(FadeCanvasGroup eventData)
        {
            // Exit case - the ID doesn't match
            if (eventData.ID != id) return;

            // Fade the Graphic
            Fade(eventData.FadeIn ? 1f : 0f, fadeDuration);

            // Raise the trigger
            eventData.Trigger.Raise();
        }

        /// <summary>
        /// Handle the Fade Tweening of the Graphic
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}

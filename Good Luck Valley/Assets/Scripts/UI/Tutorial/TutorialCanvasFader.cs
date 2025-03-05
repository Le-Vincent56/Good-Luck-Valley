using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using UnityEngine;

namespace GoodLuckValley.UI.Tutorial
{
    public class TutorialCanvasFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fields")]
        [SerializeField] private int id;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<FadeTutorialCanvas> onFadeGraphic;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            onFadeGraphic = new EventBinding<FadeTutorialCanvas>(HandleFading);
            EventBus<FadeTutorialCanvas>.Register(onFadeGraphic);
        }

        private void OnDisable()
        {
            EventBus<FadeTutorialCanvas>.Deregister(onFadeGraphic);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        private void HandleFading(FadeTutorialCanvas fadeGraphic)
        {
            // Exit case - the ID doesn't match
            if (fadeGraphic.ID != id) return;

            // Fade the Graphic
            Fade(fadeGraphic.FadeIn ? 1f : 0f, fadeDuration);

            // Raise the trigger
            fadeGraphic.Trigger.Raise();
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

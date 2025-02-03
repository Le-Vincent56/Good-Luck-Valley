using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Tutorial
{
    public class GraphicFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Graphic graphic;

        [Header("Fields")]
        [SerializeField] private int id;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<FadeGraphic> onFadeGraphic;

        private void Awake()
        {
            // Get components
            graphic = GetComponent<Graphic>();
        }

        private void OnEnable()
        {
            onFadeGraphic = new EventBinding<FadeGraphic>(HandleFading);
            EventBus<FadeGraphic>.Register(onFadeGraphic);
        }

        private void OnDisable()
        {
            EventBus<FadeGraphic>.Deregister(onFadeGraphic);
        }

        private void HandleFading(FadeGraphic fadeGraphic)
        {
            // Exit case - the ID doesn't match
            if (fadeGraphic.ID != id) return;

            // Fade the Graphic
            Fade(fadeGraphic.FadeIn ? 1f : 0f, fadeDuration);
        }

        /// <summary>
        /// Handle the Fade Tweening of the Graphic
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = graphic.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}

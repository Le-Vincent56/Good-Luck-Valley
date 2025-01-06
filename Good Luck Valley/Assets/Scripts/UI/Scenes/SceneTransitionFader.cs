using DG.Tweening;
using GoodLuckValley.Architecture.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Scenes
{
    public class SceneTransitionFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image loadingImage;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private float fadeInDuration;
        private Tween fadeTween;

        private EventBinding<FadeScene> onFadeScene;

        private void OnEnable()
        {
            onFadeScene = new EventBinding<FadeScene>(FadeScene);
            EventBus<FadeScene>.Register(onFadeScene);
        }

        private void OnDisable()
        {
            EventBus<FadeScene>.Deregister(onFadeScene);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween
            fadeTween?.Kill();
        }

        /// <summary>
        /// Callback function to handle screen fading
        /// </summary>
        private void FadeScene(FadeScene eventData)
        {
            // Check if fading in or out
            if(eventData.FadeIn)
            {
                // Fade in the loading image
                Fade(0f, fadeInDuration, eventData.EaseType, eventData.OnComplete);
            } else
            {
                // Fade out using the loading image
                Fade(1f, fadeOutDuration, eventData.EaseType, eventData.OnComplete);
            }
        }

        /// <summary>
        /// Handle the fading for loading
        /// </summary>
        private void Fade(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = loadingImage.DOFade(endValue, duration);
            fadeTween.SetEase(easeType);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}

using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Scenes
{
    public class SceneTransitionFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image loadingBackground;
        [SerializeField] private Image loadingAnari;

        [Header("Tweening Variables")]
        [SerializeField] private float backgroundOutDuration;
        [SerializeField] private float backgroundInDuration;
        [SerializeField] private float anariOutDuration;
        [SerializeField] private float anariInDuration;
        private Sequence fadeSequence;

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
            fadeSequence?.Kill();
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
                FadeSequence(0f, backgroundInDuration, anariInDuration, eventData.ShowLoadingSymbol, eventData.EaseType, eventData.OnComplete);
            } else
            {
                // Fade out using the loading image
                FadeSequence(1f, backgroundOutDuration, anariOutDuration, eventData.ShowLoadingSymbol, eventData.EaseType, eventData.OnComplete);
            }
        }

        private void FadeSequence(float endValue, float backgroundDuration, float anariDuration, bool showSymbol, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the sequence if it exists
            fadeSequence?.Kill();

            // Create the sequence
            fadeSequence = DOTween.Sequence().SetUpdate(true);

            // Fade out the loading background
            fadeSequence.Append(Fade(loadingBackground, endValue, backgroundDuration, easeType));

            // Check if showing the symbol
            if(showSymbol)
                // Fade in the loading symbol simultaneously
                fadeSequence.Join(Fade(loadingAnari, endValue, anariDuration, easeType));

            // Hook up completion actions
            fadeSequence.onComplete += onComplete;

            // Play the seqeuence
            fadeSequence.Play();
        }

        /// <summary>
        /// Handle the fading for loading
        /// </summary>
        private Tween Fade(Image image, float endValue, float duration, Ease easeType)
        {
            // Set the Fade Tween
            Tween fadeTween = image.DOFade(endValue, duration);
            fadeTween.SetEase(easeType).SetUpdate(true);

            return fadeTween;
        }
    }
}

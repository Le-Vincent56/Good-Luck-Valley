using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.Events.Player;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class CinematicBars : MonoBehaviour
    {
        [SerializeField] private RectTransform topBar;
        [SerializeField] private RectTransform bottomBar;

        [Header("Tweening Variables")]
        [SerializeField] private float initialValue;
        [SerializeField] private float translateValue;
        [SerializeField] private float translateDurationIn;
        [SerializeField] private float translateDurationOut;
        private Tween topTranslateTween;
        private Tween bottomTranslateTween;

        private EventBinding<StartCinematic> onStartCinematic;
        private EventBinding<EndCinematic> onEndCinematic;

        private void Awake()
        {
            initialValue = topBar.anchoredPosition.y;
        }

        private void OnEnable()
        {
            onStartCinematic = new EventBinding<StartCinematic>(LowerBars);
            EventBus<StartCinematic>.Register(onStartCinematic);

            onEndCinematic = new EventBinding<EndCinematic>(RaiseBars);
            EventBus<EndCinematic>.Register(onEndCinematic);
        }

        private void OnDisable()
        {
            EventBus<StartCinematic>.Deregister(onStartCinematic);
            EventBus<EndCinematic>.Deregister(onEndCinematic);
        }

        /// <summary>
        /// Lower the Cinematic Bars
        /// </summary>
        private void LowerBars()
        {
            // Deactivate the player
            EventBus<DeactivatePlayer>.Raise(new DeactivatePlayer());

            // Translate the Cinematic Bars down
            Translate(translateValue, translateDurationIn);
        }

        /// <summary>
        /// Raise the Cinematic Bars
        /// </summary>
        private void RaiseBars()
        {
            // Activate the player
            EventBus<ActivatePlayer>.Raise(new ActivatePlayer());

            // Translate the Cinematic Bars up
            Translate(initialValue, translateDurationOut);
        }

        /// <summary>
        /// Handle the translation of the Cinematic Bars
        /// </summary>
        private void Translate(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Translate Tweens if they exist
            topTranslateTween?.Kill();
            bottomTranslateTween?.Kill();

            // Set the Translate Tweens
            topTranslateTween = topBar.DOAnchorPosY(endValue, duration);
            bottomTranslateTween = bottomBar.DOAnchorPosY(-endValue, duration);

            // Set easing types
            topTranslateTween.SetEase(Ease.OutSine);
            bottomTranslateTween.SetEase(Ease.OutSine);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            topTranslateTween.onComplete += onComplete;
        }
    }
}

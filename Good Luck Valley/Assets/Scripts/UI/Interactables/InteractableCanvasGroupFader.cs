using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using UnityEngine;

namespace GoodLuckValley.UI.Interactables
{
    public class InteractableCanvasGroupFader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fields")]
        [SerializeField] private int id;

        [Header("Tweening Variables")]
        private Tween fadeTween;

        private EventBinding<FadeInteractableCanvasGroup> onFadeTutorialCanvasGroup;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            onFadeTutorialCanvasGroup = new EventBinding<FadeInteractableCanvasGroup>(HandleFading);
            EventBus<FadeInteractableCanvasGroup>.Register(onFadeTutorialCanvasGroup);
        }

        private void OnDisable()
        {
            EventBus<FadeInteractableCanvasGroup>.Deregister(onFadeTutorialCanvasGroup);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        private void HandleFading(FadeInteractableCanvasGroup eventData)
        {
            // Exit case - the ID doesn't match
            if (eventData.ID != id) return;

            // Fade the Graphic
            Fade(eventData.Value, eventData.Duration);
        }

        /// <summary>
        /// Handle the Fade Tweening of the Graphic
        /// </summary>
        private void Fade(float endValue, float duration)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, duration);
        }
    }
}

using DG.Tweening;
using GoodLuckValley.UI.Journal.Model;
using Sirenix.Reflection.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Journal.View
{
    public class TabButtonEffect : MonoBehaviour, ISelectHandler
    {
        private TabButton button;
        private RectTransform rectTransform;
        private JournalView view;

        private bool selected;
        private float initialOffset;

        [Header("Tweening Variables")]
        [SerializeField] private float translateDuration;
        [SerializeField] private Ease easeType;
        private Tween translateTween;

        public TabType Tab { get => button.Tab; }

        private void OnDestroy()
        {
            // Kill any existing Tweens
            translateTween?.Kill();
        }

        /// <summary>
        /// Initialize the Tab Button Effect
        /// </summary>
        public void Initialize(TabButton button, JournalView view)
        {
            // Get components
            rectTransform = GetComponent<RectTransform>();

            // Set the initial offset
            initialOffset = rectTransform.anchoredPosition.x;

            // Set the Journal View
            this.button = button;
            this.view = view;
        }

        /// <summary>
        /// Select the Tab Button
        /// </summary>
        public void Select()
        {
            for (int i = 0; i < view.Effects.Length; i++)
            {
                // Skip if the Tab Button Effect is this one
                if (view.Effects[i] == this) continue;

                // Deselect the Tab Button Effect
                view.Effects[i].Deselect();
            }

            // Translate out
            Translate(0f, translateDuration, easeType);

            // Set selected
            selected = true;

            // Set the last selected Tab
            view.LastSelectedTab = this;
        }

        /// <summary>
        /// Deselect the Tab Button
        /// </summary>
        public void Deselect()
        {
            // Exit case - not selected
            if (!selected) return;

            // Translate to the initial position
            Translate(initialOffset, translateDuration, easeType);

            // Set deselected
            selected = false;
        }

        /// <summary>
        /// Handle Selection events
        /// </summary>
        public void OnSelect(BaseEventData eventData) => Select();

        /// <summary>
        /// Handle Translation Tweening for the Tab Button
        /// </summary>
        private void Translate(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Translate Tween if it exists
            translateTween?.Kill();

            // Set the Translate Tween
            translateTween = rectTransform.DOAnchorPosX(endValue, duration);

            // Set the ease type
            translateTween.SetEase(easeType);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            translateTween.onComplete += onComplete;
        }
    }
}

using DG.Tweening;
using GoodLuckValley.UI.Journal.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.View
{
    public class EntryButtonEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        private JournalView view;
        private EntryButton button;
        [SerializeField] private RectTransform activeUnderline;
        [SerializeField] private RectTransform emptyUnderline;
        [SerializeField] private Text activeText;
        [SerializeField] private Text emptyText;

        [Header("Fields")]
        [SerializeField] private bool selected;

        [Header("Tweening Variables")]
        [SerializeField] private float effectDuration;
        private int defaultFontSize;
        private int selectedFontSize;
        [SerializeField] private int scaleAmount;
        [SerializeField] private Ease easeType;
        private Tween underlineTween;
        private Tween scaleTween;

        public TabType Tab { get => button.Tab; }

        private void OnDestroy()
        {
            // Kill all existing Tweens
            underlineTween?.Kill();
            scaleTween?.Kill();
        }

        /// <summary>
        /// Initialize the Entry Button Effect
        /// </summary>
        public void Initialize(EntryButton button, JournalView view)
        {
            // Set the default font size
            defaultFontSize = activeText.fontSize;
            selectedFontSize = defaultFontSize + scaleAmount;

            // Ensure the underline is hidden initially
            activeUnderline.gameObject.SetActive(false);
            emptyUnderline.gameObject.SetActive(false);

            // Set the Journal View
            this.button = button;
            this.view = view;
        }

        /// <summary>
        /// Select an active Entry Button
        /// </summary>
        public void SelectActive() => Select(activeText, activeUnderline);

        /// <summary>
        /// Selet an empty Entry Button
        /// </summary>
        public void SelectEmpty() => Select(emptyText, emptyUnderline);

        /// <summary>
        /// Select a text and underline
        /// </summary>
        public void Select(Text text, RectTransform underline)
        {
            // Get the preferred width of the text
            float textWidth = text.preferredWidth + 30f;

            // Ensure the underline is visible and animate width
            underline.gameObject.SetActive(true);

            // Set the Underline to 0 width
            underline.sizeDelta = new Vector2(0, underline.sizeDelta.y);

            // Add the Underline
            Underline(underline, new Vector2(textWidth, underline.sizeDelta.y), effectDuration, easeType);

            // Scale the text up
            Scale(text, selectedFontSize, effectDuration / 2f, easeType);

            // Set the last selected Entry
            view.LastSelectedEntry = this;
        }

        /// <summary>
        /// Deselect an active Entry Button
        /// </summary>
        public void DeselectActive() => Deselect(activeText, activeUnderline);

        /// <summary>
        /// Deselect an empty Entry Button
        /// </summary>
        public void DeselectEmpty() => Deselect(emptyText, emptyUnderline);

        /// <summary>
        /// Deselect a text and underline
        /// </summary>
        public void Deselect(Text text, RectTransform underline)
        {
            // Remove the Underline
            Underline(underline, new Vector2(0, underline.sizeDelta.y), effectDuration, easeType, () =>
            {
                // Deactivate the Underline GameObject
                underline.gameObject.SetActive(false);
            });

            // Scale the text down
            Scale(text, defaultFontSize, effectDuration / 2f, easeType);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if(button.Empty)
            {
                DeselectEmpty();
            } else
            {
                DeselectActive();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (button.Empty)
            {
                SelectEmpty();
            }
            else
            {
                SelectActive();
            }
        }

        /// <summary>
        /// Handle the Underline Tween for the Entry Button
        /// </summary>
        private void Underline(RectTransform underline, Vector2 endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Underline Tween if it exists
            underlineTween?.Kill();

            // Set the Underline Tween
            underlineTween = underline.DOSizeDelta(endValue, duration);

            // Set the Ease type
            underlineTween.SetEase(easeType);

            // Ignore time scale
            underlineTween.SetUpdate(true);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            underlineTween.onComplete += onComplete;
        }

        /// <summary>
        /// Tween the font size of the Text to select
        /// </summary>
        private void Scale(Text target, int endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Select Tween if it exists
            scaleTween?.Kill();

            // Set the Select Tween
            scaleTween = DOTween.To(() => target.fontSize, x => target.fontSize = x, (int)endValue, duration);

            // Set the easing type
            scaleTween.SetEase(easeType);

            // Ignore time scale
            scaleTween.SetUpdate(true);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            scaleTween.onComplete += onComplete;
        }
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class SelectLabel : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [Space(10), Header("References")]
        [SerializeField] private Text textToSelect;

        [Header("Default Variables")]
        [SerializeField] private int defaultFontSize;
        [SerializeField] private Color defaultColor;

        [Header("Select Variables")]
        [SerializeField] private int selectedFontSize;
        [SerializeField] private Color selectedColor;

        [Header("Changed Variables")]
        [SerializeField] private int changedFontSize;

        [Header("Tweening Variables")]
        [SerializeField] private float selectDuration;
        [SerializeField] private float deslectDuration;
        [SerializeField] private float changeDuration;
        private Tween scaleTween;
        private Tween colorTween;

        private void Awake()
        {
            // Calculate font sizes
            defaultFontSize = textToSelect.fontSize;
            selectedFontSize = defaultFontSize + 8;
            changedFontSize = defaultFontSize - 2;
        }

        private void OnDestroy()
        {
            // Kill any existing Tweens
            scaleTween?.Kill();
            colorTween?.Kill();
        }

        public void OnSelect(BaseEventData eventData)
        {
            // Select the text
            Scale(selectedFontSize, selectDuration, Ease.OutElastic);
            ChangeColor(selectedColor, selectDuration);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // Deselect the text
            Scale(defaultFontSize, deslectDuration, Ease.InOutSine);
            ChangeColor(defaultColor, deslectDuration);
        }

        public void OnChange(float value)
        {
            // Submit the text
            Scale(changedFontSize, changeDuration / 2f, Ease.OutBack, () =>
            {
                Scale(selectedFontSize, changeDuration, Ease.OutBack);
            });
            ChangeColor(selectedColor, changeDuration / 2f);
        }

        /// <summary>
        /// Tween the font size of the Text to select
        /// </summary>
        private void Scale(int endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Select Tween if it exists
            scaleTween?.Kill();

            // Set the Select Tween
            scaleTween = DOTween.To(() => textToSelect.fontSize, x => textToSelect.fontSize = x, (int)endValue, duration);

            // Set the easing type
            scaleTween.SetEase(easeType);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            scaleTween.onComplete += onComplete;
        }

        /// <summary>
        /// Tween the color of the Text to the select
        /// </summary>
        private void ChangeColor(Color endColor, float duration, TweenCallback onComplete = null)
        {
            // Kill the Color Tween if it exists
            colorTween?.Kill();

            // Set the Color Tween
            colorTween = textToSelect.DOColor(endColor, duration);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            colorTween.onComplete += onComplete;
        }
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus
{
    public class WarningText : MonoBehaviour
    {
        private Text text;

        [Header("Default Variables")]
        [SerializeField] private int defaultFontSize;
        [SerializeField] private Color defaultColor;

        [Header("Pop Variables")]
        [SerializeField] private int popFontSize;
        [SerializeField] private Color popColor;

        [Header("Tweening Variables")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float fadeDuration;
        [SerializeField] private float popDuration;
        private Tween fadeTween;
        private Tween scaleTween;
        private Tween colorTween;

        private void Awake()
        {
            // Get components
            text = GetComponent<Text>();

            // Calculate font sizes
            defaultFontSize = text.fontSize;
            popFontSize = defaultFontSize + 8;

            // Set color
            defaultColor = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        public void Pop()
        {
            // Submit the text
            Scale(popFontSize, popDuration / 2f, Ease.InOutSine, () =>
            {
                Scale(defaultFontSize, popDuration / 2f, Ease.InOutSine);
            });
            ChangeColor(popColor, popDuration / 2f, Ease.InOutSine, () =>
            {
                ChangeColor(defaultColor, popDuration / 2f, Ease.InOutSine);
            });
        }

        /// <summary>
        /// Show the Warning Text
        /// </summary>
        public void Show() => Fade(1f, fadeDuration, easeType);

        /// <summary>
        /// Hide the Warning Text
        /// </summary>
        public void Hide(bool instant = false) 
        {
            // Check if hiding out instantly
            if(instant)
            {
                // Fade out instantly
                Fade(0f, 0f, Ease.Unset);

                return;
            }

            // Fade using the fade duration and easing
            Fade(0f, fadeDuration, easeType);
        }

        /// <summary>
        /// Handle Fade Tweening for the Text
        /// </summary>
        private void Fade(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = text.DOFade(endValue, duration);

            // Set the Ease type
            fadeTween.SetEase(easeType);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Tween the font size of the Text to select
        /// </summary>
        private void Scale(int endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Select Tween if it exists
            scaleTween?.Kill();

            // Set the Select Tween
            scaleTween = DOTween.To(() => text.fontSize, x => text.fontSize = x, (int)endValue, duration);

            // Set the easing type
            scaleTween.SetEase(easeType);

            // Ignore time scale
            scaleTween.SetUpdate(true);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            scaleTween.onComplete += onComplete;
        }

        /// <summary>
        /// Tween the color of the Text to the select
        /// </summary>
        private void ChangeColor(Color endColor, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Color Tween if it exists
            colorTween?.Kill();

            // Set the Color Tween
            colorTween = text.DOColor(endColor, duration).SetUpdate(true);

            // Set the easing type
            colorTween.SetEase(easeType);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            colorTween.onComplete += onComplete;
        }
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus
{
    public class MenuButton : Selectable, ISubmitHandler
    {
        [Space(10), Header("References")]
        [SerializeField] private Text textToSelect;

        [Header("Default Variables")]
        [SerializeField] private int defaultFontSize;
        [SerializeField] private Color defaultColor;

        [Header("Select Variables")]
        [SerializeField] private int selectedFontSize;
        [SerializeField] private Color selectedColor;

        [Header("Submit Variables")]
        [SerializeField] private int submittedFontSize;

        [Header("Tweening Variables")]
        [SerializeField] private float selectDuration;
        [SerializeField] private float deslectDuration;
        [SerializeField] private float submitDuration;
        private Tween scaleTween;
        private Tween colorTween;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event hoverSound;
        [SerializeField] private AK.Wwise.Event selectSound;

        public UnityEvent OnClick = new UnityEvent();

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Calculate font sizes
            defaultFontSize = textToSelect.fontSize;
            selectedFontSize = defaultFontSize + 8;
            submittedFontSize = defaultFontSize - 2;
        }

        protected override void OnDestroy()
        {
            // Call the parent OnDestroy()
            base.OnDestroy();

            // Kill any existing Tweens
            scaleTween.Kill();
            colorTween.Kill();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            // Call the parent OnSelect()
            base.OnSelect(eventData);

            // Select the text
            Scale(selectedFontSize, selectDuration, Ease.OutElastic);
            ChangeColor(selectedColor, selectDuration);

            // Play the hover sound
            hoverSound.Post(gameObject);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            // Call the parent OnDeselect()
            base.OnDeselect(eventData);

            // Deselect the text
            Scale(defaultFontSize, deslectDuration, Ease.InOutSine);
            ChangeColor(defaultColor, deslectDuration);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            // Play the enter sound
            selectSound.Post(gameObject);

            // Submit the text
            Scale(submittedFontSize, submitDuration / 2f, Ease.OutBack, () =>
            {
                Scale(selectedFontSize, submitDuration, Ease.OutBack, () =>
                {
                    OnClick.Invoke();
                });
            });
            ChangeColor(selectedColor, submitDuration / 2f);
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
        private void ChangeColor(Color endColor, float duration, TweenCallback onComplete = null)
        {
            // Kill the Color Tween if it exists
            colorTween?.Kill();

            // Set the Color Tween
            colorTween = textToSelect.DOColor(endColor, duration).SetUpdate(true);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            colorTween.onComplete += onComplete;
        }
    }
}

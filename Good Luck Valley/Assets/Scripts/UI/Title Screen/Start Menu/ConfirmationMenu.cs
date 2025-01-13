using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class ConfirmationMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text displayText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Activate the Confirmation Menu
        /// </summary>
        public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
        {
            // Set the display text
            this.displayText.text = displayText;

            // Remove any existening listeners just to make sure there aren't any previous ones hanging around
            // This only removes listeners added through code
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            // Assign the onClick listeners
            confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu(true);
                confirmAction();
            });

            cancelButton.onClick.AddListener(() =>
            {
                DeactivateMenu(false);
                cancelAction();
            });
        }

        /// <summary>
        /// Deactivate the Confirmation Menu
        /// </summary>
        public void DeactivateMenu(bool enter)
        {
            //if (enter)
            //    // Play the button enter sound
            //    playButtonEnter.Post(gameObject);
            //else
            //    // Play the button exit sound
            //    playButtonExit.Post(gameObject);

            // Hide the Confirmation Menu
            Hide();
        }

        /// <summary>
        /// Show the Confirmation Menu
        /// </summary>
        public void Show()
        {
            Fade(1f, fadeDuration, () =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
        }

        /// <summary>
        /// Hide the Confirmation Menu
        /// </summary>
        public void Hide()
        {
            Fade(0f, fadeDuration, () =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
        }


        /// <summary>
        /// Handle Fade Tweening for the Confirmation Menu
        /// </summary>
        private void Fade(float endValue, float fadeDuration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, fadeDuration);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}

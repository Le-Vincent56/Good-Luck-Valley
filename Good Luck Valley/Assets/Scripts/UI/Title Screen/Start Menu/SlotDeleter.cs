using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu
{
    public class SlotDeleter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private SaveSlot saveSlot;
        [SerializeField] private Image image;
        [SerializeField] private Selectable selectable;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        /// <summary>
        /// Initialize the Slot Deleter
        /// </summary>
        public void Initialize(StartMenuController controller, SaveSlot saveSlot)
        {
            // Get components
            selectable = GetComponent<Selectable>();
            image = GetComponent<Image>();

            // Set variables
            this.controller = controller;
            this.saveSlot = saveSlot;

            // Don't allow interaction with the Selectable
            selectable.interactable = true;
        }

        /// <summary>
        ///  Set whether or not the Selectable can be interacted with
        /// </summary>
        public void SetSelectable(bool selectable) => this.selectable.interactable = selectable;

        public void Show()
        {

        }

        public void Hide()
        {

        }

        /// <summary>
        /// Handle Fade Tweening for the Slot Deleter's Image
        /// </summary>
        private void Fade(float endValue, float fadeDuration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = image.DOFade(endValue, fadeDuration);

            // Exit case - there is no completion action
            if(onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete = onComplete;
        }
    }
}

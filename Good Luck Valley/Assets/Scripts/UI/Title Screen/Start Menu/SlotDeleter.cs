using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.Start
{
    public class SlotDeleter : Selectable, ISelectHandler, ISubmitHandler
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private SaveSlot saveSlot;
        [SerializeField] private Image deleterImage;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        /// <summary>
        /// Initialize the Slot Deleter
        /// </summary>
        public void Initialize(StartMenuController controller, SaveSlot saveSlot)
        {
            deleterImage = GetComponent<Image>();

            // Set variables
            this.controller = controller;
            this.saveSlot = saveSlot;
        }

        /// <summary>
        ///  Set whether or not the Selectable can be interacted with
        /// </summary>
        public void SetSelectable(bool selectable) => interactable = selectable;

        /// <summary>
        /// Show the Save Slot Deleter
        /// </summary>
        public void Show()
        {
            // Exit case - If the save slot is empty
            if (saveSlot.IsEmpty)
            {
                // Hide the Save Slot
                Hide();
                return;
            }

            // Fade in
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Hide the Save Slot Deleter
        /// </summary>
        public void Hide() => Fade(0f, fadeDuration);

        /// <summary>
        /// Handle Fade Tweening for the Slot Deleter's Image
        /// </summary>
        private void Fade(float endValue, float fadeDuration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = deleterImage.DOFade(endValue, fadeDuration);

            // Exit case - there is no completion action
            if(onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete = onComplete;
        }

        /// <summary>
        /// Delete the associated Save Slot data
        /// </summary>
        public void DeleteSaveData()
        {
            // Play the button general sound
            //playButtonGeneral.Post(gameObject);

            // Activate the delete popup
            controller.ActivateDeletePopup();
        }

        /// <summary>
        /// Handle selecting the Slot Deleter
        /// </summary>
        public override void OnSelect(BaseEventData eventData)
        {
            // Exit case - not interactable
            if (!interactable)
            {
                // Delay the select of the Save Slot until the end of the frame
                StartCoroutine(DelaySelect());
                return;
            }

            // Call the parent OnSelect()
            base.OnSelect(eventData);

            // Ensure that the save slot is selected
            controller.SetSelectedSlot(saveSlot);

            // Show the Slot Deleter
            Fade(1f, 0f);
        }

        /// <summary>
        /// Handle submitting the Slot Deleter
        /// </summary>
        public void OnSubmit(BaseEventData eventData)
        {
            // Eixt case - not interactable
            if (!interactable) return;

            // Delete the game data
            DeleteSaveData();
        }

        // Delay the select until the end of the frame.
        // If we do not, the current object will be selected instead.
        private IEnumerator DelaySelect()
        {
            yield return new WaitForEndOfFrame();

            EventSystem.current.SetSelectedGameObject(saveSlot.gameObject);
        }
    }
}

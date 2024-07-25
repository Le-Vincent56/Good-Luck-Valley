using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Start
{
    public class SlotDeleter : MonoBehaviour, ISelectHandler
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private SaveSlot saveSlot;
        [SerializeField] private Image image;
        [SerializeField] private Selectable selectable;

        public void Init(SaveSlot saveSlot, StartMenuController controller)
        {
            // Get components
            selectable = GetComponent<Selectable>();
            image = GetComponent<Image>();

            // Set data
            this.controller = controller;
            this.saveSlot = saveSlot;

            // Check if the save slot is empty
            if (saveSlot.IsEmpty)
                // If so, there's no data to delete so do not allow the selectable to be interacted with
                selectable.enabled = false;
            else
                // Otherwise, there is data that can be deleted, so allow button interaction
                selectable.enabled = true;

            // Hide the image
            Hide();
        }

        /// <summary>
        /// Set whether or not the selectable is enabled
        /// </summary>
        /// <param name="selectable"></param>
        public void SetSelectable(bool selectable) => this.selectable.enabled = selectable;

        /// <summary>
        /// Delete the associated Save Slot data
        /// </summary>
        public void DeleteGame() => controller.ActivateDeletePopup();

        /// <summary>
        /// Show the Slot Delete image
        /// </summary>
        public void Show()
        {
            // Exit case - If the save slot is empty
            if (saveSlot.IsEmpty) return;

            Color currentColor = image.color;
            currentColor.a = 1f;
            image.color = currentColor;
        }

        /// <summary>
        /// Hide the Slot Delete image
        /// </summary>
        public void Hide()
        {
            Color currentColor = image.color;
            currentColor.a = 0f;
            image.color = currentColor;
        }

        /// <summary>
        /// Handle select events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            // Make sure that the image is shown
            Show();

            // Ensure that the save slot is selected
            controller.SetSelectedSlot(saveSlot);
        }
    }
}
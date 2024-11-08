using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Start
{
    public class SlotDeleter : MonoBehaviour, ISelectHandler
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonGeneral;

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

            selectable.interactable = false;

            // Hide the image
            Hide();
        }

        /// <summary>
        /// Set whether or not the selectable is enabled
        /// </summary>
        /// <param name="selectable"></param>
        public void SetSelectable(bool selectable) => this.selectable.interactable = selectable;

        /// <summary>
        /// Delete the associated Save Slot data
        /// </summary>
        public void DeleteGame()
        {
            // Play the button general sound
            playButtonGeneral.Post(gameObject);

            // Activate the delete popup
            controller.ActivateDeletePopup();
        }

        /// <summary>
        /// Show the Slot Delete image
        /// </summary>
        public void Show()
        {
            // Exit case - If the save slot is empty
            if (saveSlot.IsEmpty)
            {
                Hide();
                return;
            }

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

            // If selected, then set enabled to true
            selectable.interactable = true;
        }
    }
}
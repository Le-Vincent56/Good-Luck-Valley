using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu
{
    public class SaveSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private CanvasGroup withData;
        [SerializeField] private CanvasGroup withoutData;
        [SerializeField] private CanvasGroup selectedGroup;
        [SerializeField] private Text timeText;
        [SerializeField] private Text notesText;
        [SerializeField] private SlotDeleter deleter;

        [Header("Fields")]
        [SerializeField] private int slot;
        [SerializeField] private string saveName;
        [SerializeField] private bool isEmpty;
        [SerializeField] private bool active;

        [Header("Tweening Variables")]
        [SerializeField] private float unselectedAlpha = 0.4627f;
        [SerializeField] private float highlightDuration;

        public int Slot { get => slot; }
        public string Name { get => saveName; set => saveName = value; }
        public bool IsEmpty { get => isEmpty; }

        /// <summary>
        /// Initialize the Save Slot
        /// </summary>
        public void Initialize(StartMenuController controller)
        {
            // Set variables
            this.controller = controller;
            active = true;

            // Initialize the Slot Deleter
            deleter.Initialize(controller, this);
        }

        /// <summary>
        /// Enable the button
        /// </summary>
        public void Enable() => active = true;

        /// <summary>
        /// Disable the button
        /// </summary>
        public void Disable() => active = false;

        /// <summary>
        /// Set the Save Slot data
        /// </summary>
        public void SetData(string playTime, string notesPercent)
        {
            // Set the text
            timeText.text = playTime;
            notesText.text = notesPercent;

            // Update the UI
            UpdateUI();
        }

        public void UpdateUI()
        {
            // Set the data as valid by default
            bool validData = true;

            // Check if any data was not valid
            if (string.IsNullOrEmpty(timeText.text)) validData = false;
            if (string.IsNullOrEmpty(notesText.text)) validData = false;

            if(validData)
            {
                // Set the selected group
                selectedGroup = withData;

                withoutData.alpha = 0;
                withData.alpha = unselectedAlpha;

                // Switch to the "Without Data" object
                //withoutDataObject.SetActive(false);
                //withDataObject.SetActive(true);

                isEmpty = false;
            } else
            {
                // Switch to the "With Data" object
                //withDataObject.SetActive(false);
                //withoutDataObject.SetActive(true);

                isEmpty = true;
            }

            // Hide the Slot Deleter
            deleter.Hide();
        }

        /// <summary>
        /// Reset the Save Slot UI
        /// </summary>
        public void ResetData()
        {
            timeText.text = string.Empty;
            notesText.text = string.Empty;
            saveName = string.Empty;
        }

        /// <summary>
        /// Start a game
        /// </summary>
        public void StartGame()
        {
            // Exit case - if the Save Slot is not active
            if (!active) return;

            // Start the game
            controller.StartGame();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // Exit case - if not active
            if (!active) return;

            controller.SetSelectedSlot(this);
            deleter.Show();
            deleter.SetSelectable(!isEmpty);
        }

        public void OnSelect(BaseEventData eventData)
        {
            // Exit case - if not active
            if (!active) return;

            deleter.Hide();
            deleter.SetSelectable(false);
        }
    }
}

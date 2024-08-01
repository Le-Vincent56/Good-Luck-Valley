using GoodLuckValley.UI.TitleScreen.Start;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class SaveSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private GameObject withDataObject;
        [SerializeField] private Text time;
        [SerializeField] private Text notes;
        [SerializeField] private GameObject withoutDataObject;
        [SerializeField] private SlotDeleter deleter;
        

        [Header("Fields")]
        [SerializeField] private int slot;
        [SerializeField] private string saveName;
        [SerializeField] private bool isEmpty;

        public GameObject WithDataObject { get { return withDataObject; } }
        public Text Time { get { return time; } }
        public Text Notes {  get { return notes; } }
        public GameObject WithoutDataObject { get { return withoutDataObject; } }
        public int Slot { get { return slot; } }
        public bool IsEmpty { get { return isEmpty; } }
        public string Name { get { return saveName; } set { saveName = value; } }

        public void Init(StartMenuController controller)
        {
            this.controller = controller;
            deleter.Init(this, controller);
        }

        /// <summary>
        /// Start a game
        /// </summary>
        public void StartGame() => controller.StartGame();

        /// <summary>
        /// Set the UI Data
        /// </summary>
        /// <param name="saveName">The name of the Save Slot</param>
        /// <param name="lastUpdated">When the Save Slot was last updated</param>
        /// <param name="levelName">The current level of the Save Slot</param>
        public void SetData(string playTime, string notesPercent)
        {
            // Set text
            time.text = playTime;
            notes.text = notesPercent;

            // Update the UI
            UpdateUI();
        }

        /// <summary>
        /// Update the Save Slot UI
        /// </summary>
        public void UpdateUI()
        {
            bool validData = true;

            // Check if any data was not sent
            if (string.IsNullOrEmpty(time.text)) validData = false;
            if (string.IsNullOrEmpty(notes.text)) validData = false;

            // Check which data to set
            if (validData)
            {
                // Switch to the "Without Data" object
                withoutDataObject.SetActive(false);
                withDataObject.SetActive(true);

                isEmpty = false;
            }
            else
            {
                // Switch to the "With Data" object
                withDataObject.SetActive(false);
                withoutDataObject.SetActive(true);

                isEmpty = true;
            }
        }

        /// <summary>
        /// Reset the Save Slot UI
        /// </summary>
        public void ResetData()
        {
            time.text = string.Empty;
            notes.text = string.Empty;
            saveName = string.Empty;
        }

        /// <summary>
        /// Handle select events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            controller.SetSelectedSlot(this);
            deleter.Show();
            deleter.SetSelectable(!isEmpty);
        }

        /// <summary>
        /// Handle deselect events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData)
        {
            controller.SetSelectedSlot(null);
            deleter.Hide();
            deleter.SetSelectable(false);
        }
    }
}
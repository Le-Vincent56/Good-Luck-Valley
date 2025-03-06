using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Audio;
using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Menus.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Menus.Start
{
    public class StartMenuController : MonoBehaviour, IMenuController
    {
        [Header("References")]
        private MainMenuController mainMenuController;
        [SerializeField] private UIInputReader inputReader;
        [SerializeField] private DeleteOverlay deleteOverlay;
        [SerializeField] private SaveSlot selectedSlot;
        [SerializeField] private List<SaveSlot> saveSlots;
        private SaveLoadSystem saveLoadSystem;

        [Header("Wwise References")]
        [SerializeField] private AK.Wwise.State gameState;

        public SaveSlot SelectedSlot { get => selectedSlot; }

        private void Awake()
        {
            saveSlots = new List<SaveSlot>();

            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
            GetComponentsInChildren(saveSlots);

            // Iterate through each Save Slot
            foreach(SaveSlot slot in saveSlots)
            {
                // Initialize the Save Slot
                slot.Initialize(this);
            }

            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
        }

        private void Start()
        {
            // Set the Save Slot Data
            SetSlotData();
        }

        /// <summary>
        /// Set the selected save slot
        /// </summary>
        public void SetSelectedSlot(SaveSlot saveSlot) => selectedSlot = saveSlot;

        /// <summary>
        /// Set the Slot Data for each Save Slot
        /// </summary>
        public void SetSlotData()
        {
            // Refresh the save data
           saveLoadSystem.RefreshSaveData();

            // Store all datas into a list
            List<GameData> saveDatas = saveLoadSystem.Saves.Values.ToList();

            // Check if any data was loadded
            if (saveDatas.Count > 0)
            {
                // Loop through the save slots
                for (int i = 0; i < saveSlots.Count; i++)
                {
                    // Default to empty slots
                    saveSlots[i].ResetData();
                    saveSlots[i].UpdateUI();

                    // Loop through all of the datas
                    for (int j = 0; j < saveDatas.Count; j++)
                    {
                        // Check if the slots align
                        if (saveSlots[i].Slot == saveDatas[j].Slot)
                        {
                            // Parse date
                            DateTime saveDate = DateTime.FromBinary(saveDatas[j].LastUpdated);
                            string dateString = saveDate.ToString();

                            // Get the percentage of notes collected
                            //int notesCollected = saveDatas[j].journalSaveData.journalEntriesUnlocked;
                            //int notesTotal = 12;
                            //int percent = notesCollected / notesTotal;

                            // Set the slot name
                            saveSlots[saveDatas[j].Slot - 1].Name = saveDatas[j].Name;

                            // Build the percentage string
                            StringBuilder sb = new StringBuilder();
                            sb.Append(100);
                            sb.Append("%");

                            // Set the data
                            saveSlots[saveDatas[j].Slot - 1].SetData(dateString, sb.ToString());
                        }
                    }
                }
            }
            else
            {
                // Update all the save slots to be empty
                for (int i = 0; i < saveSlots.Count; i++)
                {
                    // Reset the data and update the UI
                    saveSlots[i].ResetData();
                    saveSlots[i].UpdateUI();
                }
            }
        }

        /// <summary>
        /// Handle the Start Button
        /// </summary>
        public void StartGame()
        {
            // Exit case - there is no selected Save Slot
            if (selectedSlot == null) return;

            // Disable the input reader
            inputReader.Disable();

            // Disable the other save slots
            foreach (SaveSlot saveSlot in saveSlots)
            {
                // Skip over the selected slot
                if (saveSlot == selectedSlot) continue;

                // Disable the Save Slot
                saveSlot.Disable();
            }

            // Stop the music
            MusicManager.Instance.SetState(gameState);

            // Check if the selected Save Slot is empty
            if (selectedSlot.IsEmpty)
                // Start a new game with new data
                NewData(selectedSlot);
            else
                // Otherwise, load the game with the Save Slot data
                LoadData(selectedSlot);
        }

        /// <summary>
        /// Create a new game within a Save Slot
        /// </summary>
        /// <param name="saveSlot">The Save Slot to start a new game in</param>
        public void NewData(SaveSlot saveSlot)
        {
            // Validate that the Save Slot exists
            if (saveSlots.Contains(saveSlot))
            {
                // Start a new game
                saveLoadSystem.NewGame(saveSlot.Slot, true);
            }
        }

        /// <summary>
        /// Load data wtihin a Save Slot
        /// </summary>
        /// <param name="saveSlot">The Save Slot to load data from</param>
        public void LoadData(SaveSlot saveSlot)
        {
            // Validate that the Save Slot exists
            if (saveSlots.Contains(saveSlot))
            {
                // Load the save file
                saveLoadSystem.LoadGame(saveSlot.Name);
            }
        }

        /// <summary>
        /// Activate the delete popup
        /// </summary>
        public void ActivateDeletePopup()
        {
            // Set the state of the delete controller
            deleteOverlay.SetState(deleteOverlay.POPUP);

            // Disable all the save slots
            foreach (SaveSlot slot in saveSlots)
            {
                slot.Disable();
            }
        }

        /// <summary>
        /// Delete data within a Save Slot
        /// </summary>
        /// <param name="saveSlot">The Save Slot to delete data from</param>
        public void DeleteData(SaveSlot saveSlot)
        {
            // Disable the input
            inputReader.Disable();
            EventSystem.current.sendNavigationEvents = false;

            // Delete the save data
            saveLoadSystem.DeleteGame(saveSlot.Name);

            // Star the delete delay
            StartCoroutine(DeleteDelay());
        }

        /// <summary>
        /// Coroutine to simulate a delay for deletion
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeleteDelay()
        {
            yield return new WaitForSecondsRealtime(1.66730f);

            // Play sound

            yield return new WaitForSecondsRealtime(1.3327f);

            // Set deleting to false
            deleteOverlay.SetState(deleteOverlay.IDLE);

            // Reset the slot data to reflect changes
            SetSlotData();

            // Set and enable save slots
            SetAndEnableSlots();

            // Re-enable input
            inputReader.Enable();
            EventSystem.current.sendNavigationEvents = true;
        }

        /// <summary>
        /// Set and Enable Save Slots
        /// </summary>
        public void SetAndEnableSlots(bool selectSlot = true)
        {
            // Choose what to select (either the Selected Slot or its Slot Deleter)
            GameObject objectToSelect = !selectSlot && !selectedSlot.IsEmpty ? selectedSlot.Deleter.gameObject : selectedSlot.gameObject;

            // Move the cursor off of the delete button andd back to the slot
            EventSystem.current.SetSelectedGameObject(objectToSelect);

            // Enable the save slots
            foreach (SaveSlot saveSlot in saveSlots)
            {
                saveSlot.Enable();
            }
        }

        /// <summary>
        /// Leave the Start menu
        /// </summary>
        public void Back()
        {
            // Exit case - currently deleting
            if (deleteOverlay.State == deleteOverlay.DELETING) return;

            // Exit case - in the delete popup
            if (deleteOverlay.State == deleteOverlay.POPUP)
            {
                // Set the state back to idle
                deleteOverlay.SetState(deleteOverlay.IDLE);

                deleteOverlay.ReturnToSlotDeleter();
                return;
            }

            // Go back to the main menu
            mainMenuController.SetState(mainMenuController.INITIAL);
        }
    }
}

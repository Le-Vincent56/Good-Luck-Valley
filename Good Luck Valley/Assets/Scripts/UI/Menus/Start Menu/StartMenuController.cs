using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Menus.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public SaveSlot SelectedSlot { get => selectedSlot; }

        private void Awake()
        {
            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
            saveSlots = GetComponentsInChildren<SaveSlot>().ToList();

            // Iterate through each Save Slot
            foreach(SaveSlot slot in saveSlots)
            {
                // Initialize the Save Slot
                slot.Initialize(this);
            }
        }

        private void Start()
        {
            // Set the Save Slot Data
            SetSlotData();
        }

        /// <summary>
        /// Set the selected save slot
        /// </summary>
        /// <param name="saveSlot"></param>
        public void SetSelectedSlot(SaveSlot saveSlot) => selectedSlot = saveSlot;

        /// <summary>
        /// Set the Slot Data for each Save Slot
        /// </summary>
        public void SetSlotData()
        {
            // Refresh the save data
            SaveLoadSystem.Instance.RefreshSaveData();

            // Store all datas into a list
            List<GameData> saveDatas = SaveLoadSystem.Instance.Saves.Values.ToList();

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

                            // Set the data
                            saveSlots[saveDatas[j].Slot - 1].SetData(dateString, $"{100}%");
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

            // Disable the other save slots
            foreach (SaveSlot saveSlot in saveSlots)
            {
                // Skip over the selected slot
                if (saveSlot == selectedSlot) continue;

                // Disable the Save Slot
                saveSlot.Disable();
            }

            // Disable the current event system
            EventSystem.current.enabled = false;

            // Set game states
            //MusicManager.Instance.SetGameStates();

            // Check if the selected Save Slot is empty
            if (selectedSlot.IsEmpty)
                // Start a new game with new data
                NewData(selectedSlot);
            // Otherwise, load the game with the Save Slot data
            else
                LoadData(selectedSlot);

            // Play enter sound
            //playButtonEnter.Post(gameObject);
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
                SaveLoadSystem.Instance.NewGame(saveSlot.Slot, true);
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
                SaveLoadSystem.Instance.LoadGame(saveSlot.Name);
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
            SaveLoadSystem.Instance.DeleteGame(saveSlot.Name);

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

            // Move the cursor off of the delete button andd back to the slot
            EventSystem.current.SetSelectedGameObject(selectedSlot.gameObject);

            // Enable the save slots
            foreach (SaveSlot saveSlot in saveSlots)
            {
                saveSlot.Enable();
            }

            // Re-enable input
            inputReader.Enable();
            EventSystem.current.sendNavigationEvents = true;
        }

        /// <summary>
        /// Leave the Start menu
        /// </summary>
        public void Back() => mainMenuController.SetState(mainMenuController.INITIAL);
    }
}

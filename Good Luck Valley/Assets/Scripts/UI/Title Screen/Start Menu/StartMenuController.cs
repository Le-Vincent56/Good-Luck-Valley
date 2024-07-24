using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using GoodLuckValley.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.TitleScreen.Start
{
    public class StartMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MenuInputReader inputReader;
        [SerializeField] private TitleScreenController controller;
        [SerializeField] private DeleteOverlayController deleteController;
        [SerializeField] private SaveSlot selectedSlot;

        [Header("Fields")]
        [SerializeField] private List<SaveSlot> saveSlots;

        public SaveSlot SelectedSlot {  get { return selectedSlot; } } 

        private void Awake()
        {
            //Get components
            controller = GetComponentInParent<TitleScreenController>();
            deleteController= GetComponent<DeleteOverlayController>();
            saveSlots = GetComponentsInChildren<SaveSlot>(true).ToList();

            // Set slot data
            SetSlotData();

            // Initialize each save slot
            foreach (SaveSlot slot in saveSlots)
            {
                slot.Init(this);
            }
        }

        /// <summary>
        /// Return to the Main Menu
        /// </summary>
        public void ReturnToMain() => controller.SetState(controller.MAIN);

        public void SetSelectedSlot(SaveSlot saveSlot) => selectedSlot = saveSlot;

        /// <summary>
        /// Set the slot data
        /// </summary>
        public void SetSlotData()
        {
            // Store all datas into a list
            List<GameData> saveDatas = SaveLoadSystem.Instance.Saves.Values.ToList();

            // Check if any data was loadded
            if (saveDatas.Count > 0)
            {
                // Loop through the Save Slots
                for (int i = 0; i < saveSlots.Count; i++)
                {
                    // Guard against if the number of save slots if higher than
                    // then number of GameDatas
                    if (i < saveDatas.Count)
                    {
                        // Parse date
                        DateTime saveDate = DateTime.FromBinary(saveDatas[i].LastUpdated);
                        string dateString = saveDate.ToString();

                        int notesCollected = saveDatas[i].journalSaveData.journalEntriesUnlocked;
                        int notesTotal = 12;
                        int percent = notesCollected / notesTotal;

                        // TODO: Set data
                        saveSlots[i].Name = saveDatas[i].Name;

                        saveSlots[i].SetData(dateString, $"{percent}%");
                    }
                    else
                    {
                        // Update UI for empty slots
                        saveSlots[i].ResetData();
                        saveSlots[i].UpdateUI();
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
            // If no slot is selected, return
            if (selectedSlot == null) return;

            Debug.Log(selectedSlot.IsEmpty);
            // Start a new game on an empty slot
            if (selectedSlot.IsEmpty)
            {
                NewData(selectedSlot);
            }
            // Otherwise, load the game
            else
            {
                LoadData(selectedSlot);
            }
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
                SaveLoadSystem.Instance.NewGame();

                // Load the scene
                SceneLoader.Instance.EnterGame(SaveLoadSystem.Instance.selectedData.CurrentLevelName);
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

                // Load the scene
                SceneLoader.Instance.EnterGame(SaveLoadSystem.Instance.selectedData.CurrentLevelName);
            }
        }

        public void ActivateDeletePopup()
        {
            deleteController.SetState(deleteController.CONFIRMATION);
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
            deleteController.SetState(deleteController.IDLE);

            // Reset the slot data to reflect changes
            SetSlotData();

            // Re-enable input
            inputReader.Enable();
            EventSystem.current.sendNavigationEvents = true;
        }
    }
}
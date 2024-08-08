using GoodLuckValley.Audio.Music;
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
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonEnter;

        [Header("References")]
        [SerializeField] private MenuInputReader inputReader;
        [SerializeField] private TitleScreenController controller;
        [SerializeField] private DeleteOverlayController deleteController;
        [SerializeField] private SaveSlot selectedSlot;

        [Header("Fields")]
        private const int stateNum = 2;
        [SerializeField] private List<SaveSlot> saveSlots;

        public SaveSlot SelectedSlot {  get { return selectedSlot; } } 

        private void Awake()
        {
            //Get components
            controller = GetComponentInParent<TitleScreenController>();
            deleteController= GetComponent<DeleteOverlayController>();
            saveSlots = GetComponentsInChildren<SaveSlot>(true).ToList();

            // Initialize each save slot
            foreach (SaveSlot slot in saveSlots)
            {
                slot.Init(this);
            }
        }

        private void Start()
        {
            // Set slot data
            SetSlotData();
        }

        /// <summary>
        /// Handle Back input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void BackInput(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not int) return;

            // Check if the delete confirmation popup is open
            if(deleteController.GetState() == deleteController.POPUP)
            {
                // Set the popup to idle and return
                deleteController.SetState(deleteController.IDLE);
                return;
            }

            // Cast and compare data
            if ((int)data == stateNum)
            {
                ReturnToMain();
            }
        }

        /// <summary>
        /// Return to the Main Menu
        /// </summary>
        public void ReturnToMain() => controller.SetState(controller.MAIN);

        /// <summary>
        /// Set the selected save slot
        /// </summary>
        /// <param name="saveSlot"></param>
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
                // Loop through the save slots
                for(int i = 0; i < saveSlots.Count; i++)
                {
                    // Default to empty slots
                    saveSlots[i].ResetData();
                    saveSlots[i].UpdateUI();

                    // Loop through all of the datas
                    for(int j = 0; j < saveDatas.Count; j++)
                    {
                        // Check if the slots align
                        if (saveSlots[i].Slot == saveDatas[j].Slot)
                        {
                            // Parse date
                            DateTime saveDate = DateTime.FromBinary(saveDatas[j].LastUpdated);
                            string dateString = saveDate.ToString();

                            // Get the percentage of notes collected
                            int notesCollected = saveDatas[j].journalSaveData.journalEntriesUnlocked;
                            int notesTotal = 12;
                            int percent = notesCollected / notesTotal;

                            // Set the slot name
                            saveSlots[saveDatas[j].Slot - 1].Name = saveDatas[j].Name;

                            // Set the data
                            saveSlots[saveDatas[j].Slot - 1].SetData(dateString, $"{percent}%");
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
            // If no slot is selected, return
            if (selectedSlot == null) return;

            // Disable input
            inputReader.Disable();

            // Set game states
            MusicManager.Instance.SetGameStates();

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

            // Play enter sound
            playButtonEnter.Post(gameObject);
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
                SaveLoadSystem.Instance.NewGame(saveSlot.Slot);

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

        /// <summary>
        /// Activate the delete popup
        /// </summary>
        public void ActivateDeletePopup()
        {
            // Set the state of the delete controller
            deleteController.SetState(deleteController.POPUP);
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

            // Move the cursor off of the delete button anbd back to the slot
            EventSystem.current.SetSelectedGameObject(selectedSlot.gameObject);

            // Re-enable input
            inputReader.Enable();
            EventSystem.current.sendNavigationEvents = true;
        }
    }
}
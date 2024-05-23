using GoodLuckValley.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuLoadState : MenuState
    {
        #region REFERENCES
        LoadController loadController;
        #endregion

        #region FIELDS
        private List<SaveSlot> saveSlots = new List<SaveSlot>();
        private Dictionary<string, Button> buttons = new Dictionary<string, Button>();
        #endregion

        public MenuLoadState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject, LoadController loadController) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
            this.loadController = loadController;
        }

        public override void LogicUpdate()
        {
            switch (menu.CheckStateChange())
            {
                // Initial State
                case 1:
                    stateMachine.ChangeState(menu.MainState);
                    break;
            }
        }

        public override async void Enter()
        {
            if (!uiObject.activeSelf) uiObject.SetActive(true);

            // Set slot data
            Reload();

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            await Show();
        }

        public override async void Exit()
        {
            await Hide();

            loadController.SetSlot(null);

            uiObject.SetActive(false);
        }

        /// <summary>
        /// Reload the state
        /// </summary>
        public void Reload()
        {
            SetSlotData();
            CheckButtons(false);

            if(SaveLoadSystem.Instance.GetSaveCount() == 0)
            {
                // Update the main menu
                menu.UpdateUIFromSaveData();

                // Return to the main menu
                menu.SetState(1);
            }
            
        }

        /// <summary>
        /// Set the slot data
        /// </summary>
        public void SetSlotData()
        {
            List<GameData> saveDatas = SaveLoadSystem.Instance.Saves.Values.ToList();

            if(saveDatas.Count > 0)
            {
                for (int i = 0; i < saveSlots.Count; i++)
                {
                    // Guard against if the number of save slots if higher than
                    // then number of GameDatas
                    if (i < saveDatas.Count)
                    {
                        // Parse date
                        DateTime saveDate = DateTime.FromBinary(saveDatas[i].LastUpdated);
                        string dateString = saveDate.ToString();

                        // Set data
                        saveSlots[i].SetData(
                            saveDatas[i].Name,
                            dateString,
                            saveDatas[i].CurrentLevelName
                        );
                    }
                    else
                    {
                        // Update UI for empty slots
                        saveSlots[i].UpdateUI();
                    }
                }
            } else
            {
                // Update all the save slots to be empty
                for(int i = 0; i < saveSlots.Count; i++)
                {
                    // Reset teh data and update the UI
                    saveSlots[i].ResetData();
                    saveSlots[i].UpdateUI();
                }
            }
        }

        public void CheckButtons(bool hasData)
        {
            if(!loadController.GetIfSlotSelected())
            {
                buttons["Delete"].interactable = false;
                buttons["Start"].interactable = false;
            } else
            {
                // Check if the button has data
                if (hasData)
                {
                    // If so, enable both buttons
                    buttons["Start"].interactable = true;
                    buttons["Delete"].interactable = true;
                } else
                {
                    // If not, then only enable start
                    buttons["Delete"].interactable = false;
                    buttons["Start"].interactable = true;
                }
            }
        }

        public override void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = uiObject.GetComponentsInChildren<Image>().ToList();
            List<Text> texts = uiObject.GetComponentsInChildren<Text>().ToList();
            List<Button> buttonsAll = uiObject.GetComponentsInChildren<Button>().ToList();

            // Add ImageDatas
            foreach (Image image in images)
            {
                imageDatas.Add(new ImageData(image));
            }

            // Add TextDatas
            foreach (Text text in texts)
            {
                textDatas.Add(new TextData(text));
            }

            // Add the Start and Delete button to the dictionary
            foreach(Button button in buttonsAll)
            {
                if (button.gameObject.name == "Start Button")
                    buttons.Add("Start", button);
                else if (button.gameObject.name == "Delete Button")
                    buttons.Add("Delete", button);
            }

            // Add Save Slots
            saveSlots = uiObject.GetComponentsInChildren<SaveSlot>().ToList();
        }


        public void DeleteData(SaveSlot saveSlot)
        {
            if(saveSlots.Contains(saveSlot))
            {
                // Delete the save file
                SaveLoadSystem.Instance.DeleteGame(saveSlot.Name);

                // Set selected slot to null
                loadController.SetSlot(null);
            }
        }

       public void LoadData(SaveSlot saveSlot)
       {
            if(saveSlots.Contains(saveSlot))
            {
                // Load the save file
                SaveLoadSystem.Instance.LoadGame(saveSlot.Name);
            }
       }

       public void NewData(SaveSlot saveSlot)
        {
            if(saveSlots.Contains(saveSlot))
            {
                SaveLoadSystem.Instance.NewGame();
            }
        }
    }
}
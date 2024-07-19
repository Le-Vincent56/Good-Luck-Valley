using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuMainState : MenuState
    {
        #region REFERENCES
        private GameObject uiObjectAlt;
        private GameObject currentObject;
        #endregion

        #region FIELDS
        private List<ImageData> imageDatasAlt = new List<ImageData>();
        private List<TextData> textDatasAlt = new List<TextData>();
        #endregion

        #region PROPERTIES
        public bool UsingAlt { get; set; }
        #endregion

        public MenuMainState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject, GameObject uiObjectAlt) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
            this.uiObjectAlt = uiObjectAlt;
        }

        public override void LogicUpdate()
        {
            switch(menu.CheckStateChange())
            {
                // Initial State
                case 0:
                    stateMachine.ChangeState(menu.InitialState);
                    break;

                // New Game
                case 2:
                    SaveLoadSystem.Instance.NewGame();
                    break;

                // Load Game
                case 3:
                    stateMachine.ChangeState(menu.LoadState);
                    break;

                // Settings
                case 4:
                    stateMachine.ChangeState(menu.SettingsState);
                    break;

                // Credits
                case 5:
                    stateMachine.ChangeState(menu.CreditsState);
                    break;

                // Exit to Desktop
                case 6:
                    // Quit the game
                    Application.Quit();
                    break;

                // Continue Game
                case 7:
                    SaveLoadSystem.Instance.ContinueGame();
                    break;
            }
        }

        public override async void Enter()
        {
            // Update the object
            UpdateObject();

            if (!currentObject.activeSelf) currentObject.SetActive(true);

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            // Check if using an alternative object
            if(UsingAlt)
            {
                // Show the alternative
                await Show(imageDatasAlt, textDatasAlt);
            } else
            {
                // Show the default
                await Show();
            }
        }

        public override async void Exit()
        {
            // Check if using an alternative object
            if(UsingAlt)
            {
                // Hide the alternative
                await Hide(imageDatasAlt, textDatasAlt);
            } else
            {
                // Hide the default
                await Hide();
            }

            // De-activate the current object
            currentObject.SetActive(false);
        }

        public override void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = uiObject.GetComponentsInChildren<Image>(true).ToList();
            List<Text> texts = uiObject.GetComponentsInChildren<Text>(true).ToList();

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

            // Store alt images and texts into lists
            List<Image> imagesAlt = uiObjectAlt.GetComponentsInChildren<Image>().ToList();
            List<Text> textsAlt = uiObjectAlt.GetComponentsInChildren<Text>().ToList();

            // Add alt ImageDatas
            foreach (Image image in imagesAlt)
            {
                imageDatasAlt.Add(new ImageData(image));
            }

            // Add alt TextDatas
            foreach (Text text in textsAlt)
            {
                textDatasAlt.Add(new TextData(text));
            }
        }

        public void UpdateObject()
        {

            // Check if using alt and set the current object
            if (UsingAlt) 
                currentObject = uiObjectAlt;
            else
                currentObject = uiObject;
        }
    }
}
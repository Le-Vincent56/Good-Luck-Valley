using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenuOld
{

    public class MenuSettingsState : MenuState
    {
        //private Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        public MenuSettingsState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject)
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
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

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            await Show(imageDatas, textDatas, false);
        }

        public override async void Exit()
        {
            await Hide(imageDatas, textDatas, false);

            uiObject.SetActive(false);
        }

        public override void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = uiObject.GetComponentsInChildren<Image>(true).ToList();
            List<Text> texts = uiObject.GetComponentsInChildren<Text>(true).ToList();
            //List<Button> buttonsAll = uiObject.GetComponentsInChildren<Button>(true).ToList();

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

            //// Add the Start and Delete button to the dictionary
            //foreach (Button button in buttonsAll)
            //{
            //    if (button.gameObject.name == "Start Button")
            //        buttons.Add("Start", button);
            //    else if (button.gameObject.name == "Delete Button")
            //        buttons.Add("Delete", button);
            //}
        }
    }
}

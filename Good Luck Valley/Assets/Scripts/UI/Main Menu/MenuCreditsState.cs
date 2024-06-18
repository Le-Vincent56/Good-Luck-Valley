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
    public class MenuCreditsState : MenuState
    {
        public MenuCreditsState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject)
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
        }
    }
}
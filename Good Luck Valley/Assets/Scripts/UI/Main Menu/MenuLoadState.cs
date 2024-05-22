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
        List<SaveSlot> saveSlots = new List<SaveSlot>();

        public MenuLoadState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject) 
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

            List<GameData> saveDatas = SaveLoadSystem.Instance.Saves.Values.ToList();

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
                } else
                {
                    // Update UI for empty slots
                    saveSlots[i].UpdateUI();
                }
            }

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

            uiObject.SetActive(false);
        }

        public override void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = uiObject.GetComponentsInChildren<Image>().ToList();
            List<Text> texts = uiObject.GetComponentsInChildren<Text>().ToList();

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

            // Add Save Slots
            saveSlots = uiObject.GetComponentsInChildren<SaveSlot>().ToList();
        }
    }
}
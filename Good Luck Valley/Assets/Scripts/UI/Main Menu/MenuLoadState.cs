using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuLoadState : MenuState
    {
        public MenuLoadState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
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

            await Show();
        }

        public override async void Exit()
        {
            await Hide();

            uiObject.SetActive(false);
        }
    }
}
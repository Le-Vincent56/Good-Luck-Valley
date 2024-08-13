using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuInitialState : MenuState
    {
        public MenuInitialState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
        }

        public override void LogicUpdate()
        {
            if (menu.CheckStateChange() == 1) stateMachine.ChangeState(menu.MainState);
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
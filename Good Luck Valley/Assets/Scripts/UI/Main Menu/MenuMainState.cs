using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuMainState : MenuState
    {
        public MenuMainState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
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
                    break;

                // Credits
                case 5:
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
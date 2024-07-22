using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class StartMenuState : MenuState
    {
        private StartMenuCursor cursors;

        public StartMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, GameObject uiObject, StartMenuCursor cursors) 
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
            this.cursors = cursors;
        }

        public override async void OnEnter()
        {
            if (!uiObject.activeSelf) uiObject.SetActive(true);

            // Activate the main menu cursors
            cursors.ActivateCursors();

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            await Show();
        }

        public override async void OnExit()
        {
            // Deactivate the main menu cursors
            cursors.DeactivateCursors();

            await Hide();

            uiObject.SetActive(false);
        }
    }
}
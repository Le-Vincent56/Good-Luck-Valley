using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.States
{
    public class AudioMenuState : MenuState
    {
        public AudioMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor)
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
        }

        public override async void OnEnter()
        {
            if (!uiObject.activeSelf) uiObject.SetActive(true);

            // Activate the main menu cursors
            cursor.ActivateCursors();

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
            cursor.DeactivateCursors();

            await Hide();

            uiObject.SetActive(false);
        }
    }
}
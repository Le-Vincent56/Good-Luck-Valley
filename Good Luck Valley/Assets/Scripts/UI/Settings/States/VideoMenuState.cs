using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using UnityEngine;

namespace GoodLuckValley.UI.Settings.States
{
    public class VideoMenuState : SettingsState
    {
        public VideoMenuState(GameSettingsMenu menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor)
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
        }

        public override async void OnEnter()
        {
            if (!uiObject.activeSelf) uiObject.SetActive(true);

            // Show the main menu cursors
            cursor.ShowCursors();

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            await Show();

            // Activate the main menu cursors
            cursor.ActivateCursors();
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
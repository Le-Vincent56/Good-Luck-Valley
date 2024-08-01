using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using GoodLuckValley.UI.TitleScreen.Settings;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.States
{
    public class MainMenuState : MenuState
    {
        private TitleBackgroundFade backgroundFade;

        public MainMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor, TitleBackgroundFade backgroundFade)
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
            this.backgroundFade = backgroundFade;
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

            // If the background fade is active (coming from Settings), hide it
            if (backgroundFade.Active())
                await backgroundFade.Hide(FadeDuration * 0.75f);

            await Show();

            // Activate the main menu cursors
            cursor.ActivateCursors();

            Debug.Log("Entered Main");
        }

        public override async void OnExit()
        {
            // Deactivate the main menu cursors
            cursor.DeactivateCursors();

            await Hide();

            uiObject.SetActive(false);

            Debug.Log("Exited Main");
        }
    }
}
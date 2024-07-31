using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.States
{
    public class StartMenuState : MenuState
    {
        private TitleBackgroundFade backgroundFade;

        public StartMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor, 
            TitleBackgroundFade backgroundFade)
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
            this.backgroundFade = backgroundFade;
        }

        public override async void OnEnter()
        {
            if (!uiObject.activeSelf) uiObject.SetActive(true);

            // Show the cursors
            cursor.ShowCursors();

            // Check whether or not to fade
            if (FadeInOut)
            {
                // Make elements invisible to prepare for the fade-in
                MakeElementsInvisible();
            }

            if (!backgroundFade.Active())
                await backgroundFade.Show(FadeDuration * 0.75f);

            await Show();

            // Activate the main menu cursors
            cursor.ActivateCursors();

            Debug.Log("Entered Start");
        }

        public override async void OnExit()
        {
            // Deactivate the main menu cursors
            cursor.DeactivateCursors();

            await Hide();

            uiObject.SetActive(false);

            Debug.Log("Exited Start");
        }
    }
}
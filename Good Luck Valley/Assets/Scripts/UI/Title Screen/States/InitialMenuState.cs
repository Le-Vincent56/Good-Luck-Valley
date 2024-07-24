using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.States
{
    public class InitialMenuState : MenuState
    {
        public InitialMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor)
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
        }

        public override async void OnEnter()
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

        public override async void OnExit()
        {
            await Hide();

            uiObject.SetActive(false);
        }
    }
}
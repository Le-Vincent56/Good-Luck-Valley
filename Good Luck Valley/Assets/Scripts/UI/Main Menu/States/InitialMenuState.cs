using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class InitialMenuState : MenuState
    {
        public InitialMenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, GameObject uiObject) 
            : base(menu, stateMachine, fadeInOut, uiObject)
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
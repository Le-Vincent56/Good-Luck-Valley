using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Menus.States
{
    public class InitialPauseState : PauseMenuState
    {
        public InitialPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration)
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // Enable menu input
            controller.EnableMenuInput();

            // Set the time scale to 0
            Time.timeScale = 0f;
        }
    }
}

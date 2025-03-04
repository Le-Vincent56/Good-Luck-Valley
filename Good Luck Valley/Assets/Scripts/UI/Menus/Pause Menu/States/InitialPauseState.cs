using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Pause.States
{
    public class InitialPauseState : PauseMenuState
    {
        private readonly IMenuController menuController;

        public InitialPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, IMenuController menuController, float fadeDuration)
            : base(controller, screen, optionMenu, fadeDuration)
        {
            this.menuController = menuController;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // Set the time scale to 0
            Time.timeScale = 0f;

            // Show the pause menu background
            controller.ShowBackground();

            // Set the menu controller
            controller.SetMenuController(menuController);
        }
    }
}

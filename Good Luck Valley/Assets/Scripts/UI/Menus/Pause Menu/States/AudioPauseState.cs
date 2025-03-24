using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Pause.States
{
    public class AudioPauseState : PauseMenuState
    {
        private readonly IMenuController menuController;

        public AudioPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, IMenuController menuController, float fadeDuration)
            : base(controller, screen, optionMenu, fadeDuration)
        {
            this.menuController = menuController;
        }

        public override void OnEnter()
        {
            Fade(1f, fadeDuration, Ease.InOutSine, () =>
            {
                screen.interactable = true;
                screen.blocksRaycasts = true;

                // Enable all buttons
                optionMenu.EnableAllButtons();

                // Update the first selected of the Option Menu
                optionMenu.SelectFirst();

                // Set the current menu controller
                controller.SetMenuController(menuController);
            });
        }
    }
}

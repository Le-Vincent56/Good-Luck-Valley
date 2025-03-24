using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Main.States
{
    public class ControlsMenuState : MainMenuState
    {
        private readonly IMenuController menuController;

        public ControlsMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, IMenuController menuController, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        {
            this.menuController = menuController;
        }

        public override void OnEnter()
        {
            FadeGroup(1f, fadeDuration, Ease.InQuint, () =>
            {
                screen.interactable = true;
                screen.blocksRaycasts = true;

                // Enable all buttons
                optionMenu.EnableAllButtons();

                // Update the first selected of the Option Menu
                optionMenu.SelectFirst();

                // Set the menu controller
                controller.SetMenuController(menuController);
            });

            // Fade in the darker background
            FadeBackground(0.902f, fadeDuration, Ease.InOutSine);
        }

        public override void OnExit()
        {
            // Disable all buttons
            optionMenu.DisableAllButtons();

            FadeGroup(0f, fadeDuration, Ease.OutQuint, () =>
            {
                screen.interactable = false;
                screen.blocksRaycasts = false;
            });

            // Fade out the darker background
            FadeBackground(0f, fadeDuration, Ease.InOutSine);

            // Update the first selected of the Option Menu
            optionMenu.UpdateFirst();
        }
    }
}

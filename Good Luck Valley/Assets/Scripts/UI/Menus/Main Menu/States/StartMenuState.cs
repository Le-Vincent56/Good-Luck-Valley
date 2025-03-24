using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Main.States
{
    public class StartMenuState : MainMenuState
    {
        private readonly IMenuController menuController;

        public StartMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, IMenuController menuController, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        {
            this.menuController = menuController;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // Fade in the darker background
            FadeBackground(0.902f, fadeDuration, Ease.InOutSine);

            // Set the menu controller
            controller.SetMenuController(menuController);
        }

        public override void OnExit()
        {
            base.OnExit();

            // Fade out the darker background
            FadeBackground(0f, fadeDuration, Ease.InOutSine);
        }
    }
}

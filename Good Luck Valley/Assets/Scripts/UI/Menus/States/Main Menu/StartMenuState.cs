using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.States
{
    public class StartMenuState : MainMenuState
    {
        public StartMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // Fade in the darker background
            FadeBackground(0.902f, fadeDuration, Ease.InOutSine);
        }

        public override void OnExit()
        {
            base.OnExit();

            // Fade out the darker background
            FadeBackground(0f, fadeDuration, Ease.InOutSine);
        }
    }
}

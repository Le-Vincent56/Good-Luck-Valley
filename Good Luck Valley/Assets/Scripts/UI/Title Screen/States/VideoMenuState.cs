using DG.Tweening;
using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class VideoMenuState : MainMenuState
    {
        public VideoMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu) 
            : base(controller, screen, darkerBackground, optionMenu)
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

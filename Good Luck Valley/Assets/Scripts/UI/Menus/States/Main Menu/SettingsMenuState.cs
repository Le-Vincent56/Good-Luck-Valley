using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.States
{
    public class SettingsMenuState : MainMenuState
    {
        public SettingsMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            FadeGroup(1f, fadeDuration, Ease.InQuint, () =>
            {
                screen.interactable = true;
                screen.blocksRaycasts = true;

                // Update the first selected of the Option Menu
                optionMenu.SelectFirst();
            });

            // Fade in the darker background
            FadeBackground(0.902f, fadeDuration, Ease.InOutSine);
        }

        public override void OnExit()
        {
            FadeGroup(0f, fadeDuration, Ease.OutQuint, () =>
            {
                screen.interactable = true;
                screen.blocksRaycasts = true;

                // Update the first selected of the Option Menu
                optionMenu.SelectFirst();
            });

            // Fade out the darker background
            FadeBackground(0f, fadeDuration, Ease.InOutSine);
        }
    }
}

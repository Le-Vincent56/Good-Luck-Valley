using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.States
{
    public class VideoPauseState : PauseMenuState
    {
        public VideoPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration)
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }
    }
}

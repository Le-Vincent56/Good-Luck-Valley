using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.States;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.States
{
    public class SettingsPauseState : PauseMenuState
    {
        public SettingsPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration)
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }
    }
}

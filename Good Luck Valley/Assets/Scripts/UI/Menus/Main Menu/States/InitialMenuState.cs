using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Main.States
{
    public class InitialMenuState : MainMenuState
    {
        public InitialMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration)
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        { }
    }
}

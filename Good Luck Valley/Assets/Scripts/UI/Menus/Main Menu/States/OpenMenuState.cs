using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Main.States
{
    public class OpenMenuState : MainMenuState
    {
        public OpenMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        { }
    }
}

using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class OpenMenuState : MainMenuState
    {
        public OpenMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        { }
    }
}

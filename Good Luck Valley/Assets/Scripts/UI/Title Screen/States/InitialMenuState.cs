using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class InitialMenuState : MainMenuState
    {
        public InitialMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, darkerBackground, optionMenu, fadeDuration)
        { }
    }
}

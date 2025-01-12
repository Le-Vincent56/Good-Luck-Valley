using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class InitialMenuState : MainMenuState
    {
        public InitialMenuState(MainMenuController controller, CanvasGroup screen, IOptionMenu optionMenu) 
            : base(controller, screen, optionMenu)
        { }
    }
}

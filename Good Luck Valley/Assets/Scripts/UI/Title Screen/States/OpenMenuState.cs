using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class OpenMenuState : MainMenuState
    {
        public OpenMenuState(MainMenuController controller, CanvasGroup screen, IOptionMenu optionMenu) 
            : base(controller, screen, optionMenu)
        { }
    }
}

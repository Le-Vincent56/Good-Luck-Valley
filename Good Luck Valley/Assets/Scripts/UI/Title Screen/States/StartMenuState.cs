using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class StartMenuState : MainMenuState
    {
        public StartMenuState(MainMenuController controller, CanvasGroup screen, IOptionMenu optionMenu) 
            : base(controller, screen, optionMenu)
        {
        }
    }
}

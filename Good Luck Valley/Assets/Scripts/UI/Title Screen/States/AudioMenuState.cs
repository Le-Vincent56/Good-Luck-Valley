using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class AudioMenuState : MainMenuState
    {
        public AudioMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu) 
            : base(controller, screen, darkerBackground, optionMenu)
        {
        }
    }
}

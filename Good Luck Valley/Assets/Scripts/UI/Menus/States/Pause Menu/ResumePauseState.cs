using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class ResumePauseState : PauseMenuState
    {
        public ResumePauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {

        }
    }
}
